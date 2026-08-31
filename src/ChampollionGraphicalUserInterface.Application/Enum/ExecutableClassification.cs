namespace ChampollionGraphicalUserInterface.Application.Enums;

/// <summary>
/// Identifies the Champollion edition indicated by an executable installation.
/// </summary>
public enum ExecutableClassification
{
    /// <summary>
    /// The executable edition cannot be determined reliably.
    /// </summary>
    Unknown,

    /// <summary>
    /// The executable belongs to the legacy Champollion edition.
    /// </summary>
    Legacy,

    /// <summary>
    /// The executable belongs to the current Champollion edition.
    /// </summary>
    Current,
}