namespace ChampollionGraphicalUserInterface.Domain;

/// <summary>
/// Describes one invocation of an external Champollion executable.
/// </summary>
/// <param name="Edition">The Champollion generation to invoke.</param>
/// <param name="Game">The game format to process.</param>
/// <param name="Operation">The command-line operation to perform.</param>
/// <param name="ExecutablePath">The absolute path to the Champollion executable.</param>
/// <param name="InputPath">The optional PEX file or directory to process.</param>
/// <param name="Options">The decompilation options and output paths.</param>
public sealed record ChampollionRequest(
    ChampollionEdition Edition,
    SupportedGame Game,
    ChampollionOperation Operation,
    string ExecutablePath,
    string? InputPath,
    DecompilationOptions Options);