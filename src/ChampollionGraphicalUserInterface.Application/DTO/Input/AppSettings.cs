using ChampollionGraphicalUserInterface.Domain;

namespace ChampollionGraphicalUserInterface.Application.DTO.Input;

/// <summary>
/// Stores application-wide executable paths and game-specific option profiles.
/// </summary>
public sealed record AppSettings
{
    #region Properties

    /// <summary>
    /// Gets the saved path to the legacy Champollion executable.
    /// </summary>
    /// <value>The legacy executable path, or <see langword="null"/> when no path has been saved.</value>
    public string? LegacyExecutablePath { get; init; }

    /// <summary>
    /// Gets the saved path to the current Champollion executable.
    /// </summary>
    /// <value>The current executable path, or <see langword="null"/> when no path has been saved.</value>
    public string? CurrentExecutablePath { get; init; }

    /// <summary>
    /// Gets the most recently selected game for the legacy edition.
    /// </summary>
    /// <value>The last selected legacy game.</value>
    public SupportedGame LastLegacyGame { get; init; } = SupportedGame.Skyrim;

    /// <summary>
    /// Gets the most recently selected game for the current edition.
    /// </summary>
    /// <value>The last selected current game.</value>
    public SupportedGame LastCurrentGame { get; init; } = SupportedGame.SkyrimSpecialEdition;

    /// <summary>
    /// Gets the saved decompilation options keyed by edition and game.
    /// </summary>
    /// <value>The option profiles for each configured edition and game combination.</value>
    public Dictionary<string, SavedOptions> EditionGameOptions { get; init; } = [];

    #endregion
}