namespace ChampollionGraphicalUserInterface.Application.DTO.Output;

/// <summary>
/// Represents a chunk of output produced by a Champollion process.
/// </summary>
/// <param name="InputPath">The input path associated with the output, or <see langword="null"/> when no input is used.</param>
/// <param name="IsError">Whether the text was read from the standard error stream.</param>
/// <param name="Text">The output text.</param>
public sealed record ExecutionOutput(string? InputPath, bool IsError, string Text);