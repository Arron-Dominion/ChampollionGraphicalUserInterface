namespace ChampollionGraphicalUserInterface.Application.DTO.Output;

/// <summary>
/// Represents the result of executing Champollion for one input.
/// </summary>
/// <param name="InputPath">The processed input path, or <see langword="null"/> when no input is used.</param>
/// <param name="ExitCode">The process exit code.</param>
/// <param name="StandardOutput">The complete standard output text.</param>
/// <param name="StandardError">The complete standard error text.</param>
/// <param name="Succeeded">Whether the process completed successfully.</param>
public sealed record FileExecutionResult(
    string? InputPath,
    int ExitCode,
    string StandardOutput,
    string StandardError,
    bool Succeeded);