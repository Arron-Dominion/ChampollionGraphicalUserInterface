namespace ChampollionGraphicalUserInterface.Domain;

/// <summary>
/// Identifies the supported Champollion command-line tool generation.
/// </summary>
public enum ChampollionEdition
{
    /// <summary>
    /// The original .NET Framework-era Champollion release.
    /// </summary>
    Legacy,

    /// <summary>
    /// The current native Champollion release.
    /// </summary>
    Current,
}