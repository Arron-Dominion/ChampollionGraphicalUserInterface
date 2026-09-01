using System.Diagnostics;
using ChampollionGraphicalUserInterface.Application.CommandLine;
using ChampollionGraphicalUserInterface.Application.DTO.Output;
using ChampollionGraphicalUserInterface.Application.Validation;
using ChampollionGraphicalUserInterface.Domain;

namespace ChampollionGraphicalUserInterface.Application.Execution;

/// <summary>
/// Validates Champollion requests and executes them for each resolved input.
/// </summary>
/// <param name="pathValidator">The validator used to validate and normalize local paths.</param>
/// <param name="logWriter">The writer used to persist diagnostic logs for noteworthy results.</param>
public sealed class ChampollionRunner(LocalPathValidator pathValidator, DiagnosticLogWriter logWriter)
{
    #region Methods

    /// <summary>
    /// Executes a validated Champollion request and reports output and file progress.
    /// </summary>
    /// <param name="request">The Champollion request to execute.</param>
    /// <param name="progress">An optional receiver for completed-input progress.</param>
    /// <param name="output">An optional receiver for process output chunks.</param>
    /// <param name="cancellationToken">The token used to cancel validation and execution.</param>
    /// <returns>A task containing the aggregate execution summary.</returns>
    public async Task<ExecutionSummary> RunAsync(
        ChampollionRequest request,
        IProgress<ExecutionProgress>? progress = null,
        IProgress<ExecutionOutput>? output = null,
        CancellationToken cancellationToken = default)
    {
        List<string> validationErrors = [.. CompatibilityRules.Validate(request)];
        PathValidationResult executable = pathValidator.ValidateExecutable(request.ExecutablePath);
        if (!executable.IsValid)
        {
            validationErrors.Add(executable.Error!);
        }

        IReadOnlyList<string?> inputs = ResolveInputs(request, validationErrors);
        ValidateOutputs(request, validationErrors);
        if (validationErrors.Count > 0)
        {
            throw new ArgumentException(string.Join(Environment.NewLine, validationErrors));
        }

        ChampollionRequest normalizedRequest = NormalizePaths(request, executable.ExpandedPath!);
        CreateOutputDirectories(normalizedRequest.Options);
        List<FileExecutionResult> results = [];
        for (int index = 0; index < inputs.Count; index++)
        {
            string? input = inputs[index];
            FileExecutionResult result = await RunProcessAsync(normalizedRequest, input, output, cancellationToken);
            results.Add(result);
            progress?.Report(new ExecutionProgress(index + 1, inputs.Count, input ?? request.Operation.ToString()));
        }

        string? logPath = results.Any(result => !result.Succeeded || !string.IsNullOrWhiteSpace(result.StandardError))
            ? await logWriter.WriteAsync(normalizedRequest, results, cancellationToken)
            : null;
        return CreateExecutionSummary(results, logPath);
    }

    /// <summary>
    /// Creates a request with normalized executable and optional output paths.
    /// </summary>
    /// <param name="request">The request to normalize.</param>
    /// <param name="executablePath">The validated executable path.</param>
    /// <returns>A copy of the request containing normalized paths.</returns>
    private static ChampollionRequest NormalizePaths(ChampollionRequest request, string executablePath) => request with
    {
        ExecutablePath = executablePath,
        Options = request.Options with
        {
            SourceOutputPath = ExpandOptionalPath(request.Options.SourceOutputPath),
            AssemblyOutputPath = ExpandOptionalPath(request.Options.AssemblyOutputPath),
        },
    };

    /// <summary>
    /// Expands and normalizes an optional path.
    /// </summary>
    /// <param name="path">The optional path to expand.</param>
    /// <returns>The normalized absolute path, or <see langword="null"/> when no path is supplied.</returns>
    private static string? ExpandOptionalPath(string? path) => string.IsNullOrWhiteSpace(path)
        ? null
        : Path.GetFullPath(Environment.ExpandEnvironmentVariables(path.Trim().Trim('"')));

    /// <summary>
    /// Resolves the request input into the files that should be processed.
    /// </summary>
    /// <param name="request">The request whose input should be resolved.</param>
    /// <param name="errors">The collection that receives input validation errors.</param>
    /// <returns>The resolved input paths, or a single null entry for operations that require no input.</returns>
    private IReadOnlyList<string?> ResolveInputs(ChampollionRequest request, List<string> errors)
    {
        if (!CompatibilityRules.RequiresInput(request.Operation))
        {
            return [null];
        }

        PathValidationResult input = pathValidator.ValidateInput(request.InputPath);
        if (!input.IsValid)
        {
            errors.Add(input.Error!);
            return [];
        }

        if (File.Exists(input.ExpandedPath))
        {
            return [input.ExpandedPath];
        }

        SearchOption searchOption = request.Options.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        try
        {
            string[] files = Directory.GetFiles(input.ExpandedPath!, "*.pex", searchOption);
            if (files.Length == 0)
            {
                errors.Add("The input directory contains no matching .pex files.");
            }

            return files;
        }
        catch (UnauthorizedAccessException)
        {
            errors.Add("The input directory contains a location that cannot be read.");
            return [];
        }
    }

    /// <summary>
    /// Validates each configured output path and collects any errors.
    /// </summary>
    /// <param name="request">The request containing output paths to validate.</param>
    /// <param name="errors">The collection that receives output validation errors.</param>
    private void ValidateOutputs(ChampollionRequest request, List<string> errors)
    {
        foreach (string? outputPath in new[] { request.Options.SourceOutputPath, request.Options.AssemblyOutputPath })
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                continue;
            }

            PathValidationResult result = pathValidator.ValidateOutput(outputPath);
            if (!result.IsValid)
            {
                errors.Add(result.Error!);
            }
        }
    }

    /// <summary>
    /// Creates configured source and assembly output directories.
    /// </summary>
    /// <param name="options">The options containing output directory paths.</param>
    private static void CreateOutputDirectories(DecompilationOptions options)
    {
        foreach (string? outputPath in new[] { options.SourceOutputPath, options.AssemblyOutputPath })
        {
            if (!string.IsNullOrWhiteSpace(outputPath))
            {
                Directory.CreateDirectory(Environment.ExpandEnvironmentVariables(outputPath));
            }
        }
    }

    /// <summary>
    /// Runs one Champollion process and captures its output streams.
    /// </summary>
    /// <param name="request">The normalized request to execute.</param>
    /// <param name="inputPath">The input path for this process, or <see langword="null"/> for input-free operations.</param>
    /// <param name="output">An optional receiver for process output chunks.</param>
    /// <param name="cancellationToken">The token used to cancel process execution and stream reads.</param>
    /// <returns>A task containing the result of the process execution.</returns>
    private static async Task<FileExecutionResult> RunProcessAsync(
        ChampollionRequest request,
        string? inputPath,
        IProgress<ExecutionOutput>? output,
        CancellationToken cancellationToken)
    {
        ProcessStartInfo startInfo = new(request.ExecutablePath)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = Path.GetDirectoryName(request.ExecutablePath),
        };

        foreach (string argument in ChampollionCommandBuilder.BuildArguments(request, inputPath))
        {
            startInfo.ArgumentList.Add(argument);
        }

        using Process process = new() { StartInfo = startInfo };
        process.Start();
        Task<string> standardOutput = ReadStreamAsync(process.StandardOutput, inputPath, false, output, cancellationToken);
        Task<string> standardError = ReadStreamAsync(process.StandardError, inputPath, true, output, cancellationToken);
        await Task.WhenAll(process.WaitForExitAsync(cancellationToken), standardOutput, standardError);
        return CreateFileExecutionResult(
            inputPath,
            process.ExitCode,
            standardOutput.Result,
            standardError.Result);
    }

    /// <summary>
    /// Creates a file execution result from captured process values.
    /// </summary>
    /// <param name="inputPath">The input path processed by the process.</param>
    /// <param name="exitCode">The process exit code.</param>
    /// <param name="standardOutput">The captured standard output.</param>
    /// <param name="standardError">The captured standard error.</param>
    /// <returns>The file execution result.</returns>
    internal static FileExecutionResult CreateFileExecutionResult(
        string? inputPath,
        int exitCode,
        string standardOutput,
        string standardError) => new(inputPath, exitCode, standardOutput, standardError, exitCode == 0);

    /// <summary>
    /// Creates aggregate counts and log information for a set of execution results.
    /// </summary>
    /// <param name="results">The individual file execution results.</param>
    /// <param name="logPath">The diagnostic log path, if a log was written.</param>
    /// <returns>The aggregate execution summary.</returns>
    internal static ExecutionSummary CreateExecutionSummary(
        IReadOnlyList<FileExecutionResult> results,
        string? logPath)
    {
        int successfulCount = results.Count(result => result.Succeeded);
        return new ExecutionSummary(results, logPath, successfulCount, results.Count - successfulCount);
    }

    /// <summary>
    /// Reads a process stream to completion while forwarding each chunk to an output receiver.
    /// </summary>
    /// <param name="reader">The process stream reader.</param>
    /// <param name="inputPath">The input path associated with the process.</param>
    /// <param name="isError">Whether the reader represents the standard error stream.</param>
    /// <param name="output">An optional receiver for output chunks.</param>
    /// <param name="cancellationToken">The token used to cancel stream reading.</param>
    /// <returns>A task containing the complete stream contents.</returns>
    internal static async Task<string> ReadStreamAsync(
        StreamReader reader,
        string? inputPath,
        bool isError,
        IProgress<ExecutionOutput>? output,
        CancellationToken cancellationToken)
    {
        char[] buffer = new char[1024];
        System.Text.StringBuilder completeOutput = new();
        while (true)
        {
            int charactersRead = await reader.ReadAsync(buffer, cancellationToken);
            if (charactersRead == 0)
            {
                break;
            }

            string chunk = new(buffer, 0, charactersRead);
            completeOutput.Append(chunk);
            output?.Report(new ExecutionOutput(inputPath, isError, chunk));
        }

        return completeOutput.ToString();
    }

    #endregion
}