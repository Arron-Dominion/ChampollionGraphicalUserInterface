namespace ChampollionGraphicalUserInterface.Application.DTO.Output;

/// <summary>
/// Summarizes the results of a Champollion execution.
/// </summary>
/// <param name="Results">The result produced for each input.</param>
/// <param name="LogPath">The diagnostic log path, or <see langword="null"/> when no log was written.</param>
/// <param name="SuccessfulCount">The number of successful executions.</param>
/// <param name="FailedCount">The number of failed executions.</param>
public sealed record ExecutionSummary(
    IReadOnlyList<FileExecutionResult> Results,
    string? LogPath,
    int SuccessfulCount,
    int FailedCount);