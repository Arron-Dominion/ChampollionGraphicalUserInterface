using ChampollionGraphicalUserInterface.Domain;

namespace ChampollionGraphicalUserInterface.Application.CommandLine;

/// <summary>
/// Builds command-line arguments for Champollion requests.
/// </summary>
public static class ChampollionCommandBuilder
{
    #region Methods

    /// <summary>
    /// Builds the command-line arguments required to execute a request.
    /// </summary>
    /// <param name="request">The request whose operation and options determine the arguments.</param>
    /// <param name="inputPath">An optional input path that overrides the path stored in the request.</param>
    /// <returns>The ordered command-line arguments for the request.</returns>
    public static IReadOnlyList<string> BuildArguments(ChampollionRequest request, string? inputPath = null)
    {
        List<string> arguments = [];

        switch (request.Operation)
        {
            case ChampollionOperation.Help:
                return ["--help"];
            case ChampollionOperation.Version:
                return ["--version"];
        }

        if (!string.IsNullOrWhiteSpace(inputPath ?? request.InputPath))
        {
            arguments.Add(inputPath ?? request.InputPath!);
        }

        if (request.Operation == ChampollionOperation.PrintInformation)
        {
            arguments.Add("--print-info");
            return arguments;
        }

        if (request.Operation == ChampollionOperation.PrintCompileTime)
        {
            arguments.Add("--print-compile-time");
            return arguments;
        }

        DecompilationOptions options = request.Options;
        AddValue(arguments, "--psc", options.SourceOutputPath);

        if (options.GenerateAssembly)
        {
            arguments.Add("--asm");
            if (!string.IsNullOrWhiteSpace(options.AssemblyOutputPath))
            {
                arguments.Add(options.AssemblyOutputPath);
            }
        }

        AddFlag(arguments, options.GenerateComments, "--comment");
        AddFlag(arguments, options.Recursive, "--recursive");
        AddFlag(arguments, options.RecreateSubdirectories, "--recreate-subdirs");
        AddFlag(arguments, options.WriteHeader, "--header");
        AddFlag(arguments, options.Trace, "--trace");
        AddFlag(arguments, options.NoDumpTree, "--no-dump-tree");
        AddFlag(arguments, options.DebugFunctions, "--debug-funcs");
        AddFlag(arguments, options.NoDebugLineNumbers, "--no-debug-line");
        AddFlag(arguments, options.Verbose, "--verbose");
        return arguments;
    }

    /// <summary>
    /// Adds a command-line flag when its corresponding option is selected.
    /// </summary>
    /// <param name="arguments">The argument collection to update.</param>
    /// <param name="selected">Whether the flag should be added.</param>
    /// <param name="flag">The command-line flag to add.</param>
    private static void AddFlag(List<string> arguments, bool selected, string flag)
    {
        if (selected)
        {
            arguments.Add(flag);
        }
    }

    /// <summary>
    /// Adds a command-line flag and its nonempty value.
    /// </summary>
    /// <param name="arguments">The argument collection to update.</param>
    /// <param name="flag">The command-line flag associated with the value.</param>
    /// <param name="value">The optional value to add.</param>
    private static void AddValue(List<string> arguments, string flag, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            arguments.Add(flag);
            arguments.Add(value);
        }
    }

    #endregion
}