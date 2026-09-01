namespace ChampollionGraphicalUserInterface.Application.DTO.Output;

/// <summary>
/// Reports progress while searching directories for a Champollion executable.
/// </summary>
/// <param name="DirectoriesSearched">The number of directories whose search has started.</param>
/// <param name="ActiveWorkers">The number of workers currently searching directories.</param>
/// <param name="WorkerCount">The total number of search workers.</param>
public sealed record SearchProgress(int DirectoriesSearched, int ActiveWorkers, int WorkerCount);