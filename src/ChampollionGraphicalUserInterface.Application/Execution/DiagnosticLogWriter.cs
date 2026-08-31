using System.Text;
using ChampollionGraphicalUserInterface.Application.DTO.Output;
using ChampollionGraphicalUserInterface.Domain;

namespace ChampollionGraphicalUserInterface.Application.Execution;

/// <summary>
/// Writes diagnostic logs for Champollion executions.
/// </summary>
public sealed class DiagnosticLogWriter
{
    #region Variables

    /// <summary>
    /// The directory in which diagnostic logs are stored.
    /// </summary>
    private readonly string logDirectory;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DiagnosticLogWriter"/> class.
    /// </summary>
    /// <param name="applicationDirectory">The application directory used as the root for user data.</param>
    public DiagnosticLogWriter(string? applicationDirectory = null)
    {
        logDirectory = Path.Combine(applicationDirectory ?? AppContext.BaseDirectory, "UserData", "Logs");
    }

    #endregion

    #region Methods

    /// <summary>
    /// Writes request details and execution results to a timestamped diagnostic log.
    /// </summary>
    /// <param name="request">The request associated with the execution.</param>
    /// <param name="results">The file execution results to record.</param>
    /// <param name="cancellationToken">The token used to cancel the file write.</param>
    /// <returns>A task containing the path of the written log file.</returns>
    public async Task<string> WriteAsync(
        ChampollionRequest request,
        IReadOnlyList<FileExecutionResult> results,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(logDirectory);
        string path = Path.Combine(logDirectory, $"Champollion-{DateTimeOffset.Now:yyyyMMdd-HHmmss}.log");
        StringBuilder contents = new();
        contents.AppendLine($"Edition: {request.Edition}");
        contents.AppendLine($"Game: {request.Game}");
        contents.AppendLine($"Operation: {request.Operation}");

        foreach (FileExecutionResult result in results)
        {
            contents.AppendLine();
            contents.AppendLine($"Input: {result.InputPath ?? "(none)"}");
            contents.AppendLine($"Exit code: {result.ExitCode}");
            contents.AppendLine("Standard output:");
            contents.AppendLine(result.StandardOutput);
            contents.AppendLine("Standard error:");
            contents.AppendLine(result.StandardError);
        }

        await File.WriteAllTextAsync(path, contents.ToString(), cancellationToken);
        return path;
    }

    #endregion
}