namespace ChampollionGraphicalUserInterface.Application.DTO.Output;

/// <summary>
/// Represents the result of validating and expanding a local path.
/// </summary>
/// <param name="IsValid">Whether the path is valid for its intended use.</param>
/// <param name="ExpandedPath">The normalized absolute path, or <see langword="null"/> when validation fails.</param>
/// <param name="Error">The validation error, or <see langword="null"/> when validation succeeds.</param>
public sealed record PathValidationResult(bool IsValid, string? ExpandedPath, string? Error);