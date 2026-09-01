namespace ChampollionGraphicalUserInterface.Application.DTO.Output;

/// <summary>
/// Reports progress while processing Champollion inputs.
/// </summary>
/// <param name="Completed">The number of inputs that have completed.</param>
/// <param name="Total">The total number of inputs to process.</param>
/// <param name="CurrentInput">The input most recently processed.</param>
public sealed record ExecutionProgress(int Completed, int Total, string CurrentInput);