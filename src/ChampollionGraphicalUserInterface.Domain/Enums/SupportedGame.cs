namespace ChampollionGraphicalUserInterface.Domain;

/// <summary>
/// Identifies a game whose Papyrus executable format is supported by Champollion.
/// </summary>
public enum SupportedGame
{
    /// <summary>The original release of The Elder Scrolls V: Skyrim.</summary>
    Skyrim,

    /// <summary>The Elder Scrolls V: Skyrim Special Edition.</summary>
    SkyrimSpecialEdition,

    /// <summary>Fallout 4.</summary>
    Fallout4,

    /// <summary>Fallout 76.</summary>
    Fallout76,

    /// <summary>Starfield.</summary>
    Starfield,
}