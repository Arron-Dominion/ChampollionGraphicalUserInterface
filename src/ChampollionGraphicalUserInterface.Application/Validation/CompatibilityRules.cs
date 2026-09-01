using ChampollionGraphicalUserInterface.Domain;

namespace ChampollionGraphicalUserInterface.Application.Validation;

/// <summary>
/// Defines supported games, operations, and options for each Champollion edition.
/// </summary>
public static class CompatibilityRules
{
    #region Variables

    /// <summary>
    /// Games supported by the legacy Champollion edition.
    /// </summary>
    private static readonly IReadOnlyList<SupportedGame> LegacyGames =
        [SupportedGame.Skyrim, SupportedGame.SkyrimSpecialEdition];

    /// <summary>
    /// Games supported by the current Champollion edition.
    /// </summary>
    private static readonly IReadOnlyList<SupportedGame> CurrentGames =
        [SupportedGame.Skyrim, SupportedGame.SkyrimSpecialEdition, SupportedGame.Fallout4,
         SupportedGame.Fallout76, SupportedGame.Starfield];

    #endregion

    #region Methods

    /// <summary>
    /// Gets the games supported by a Champollion edition.
    /// </summary>
    /// <param name="edition">The Champollion edition.</param>
    /// <returns>The games supported by the edition.</returns>
    public static IReadOnlyList<SupportedGame> GamesFor(ChampollionEdition edition) =>
        edition == ChampollionEdition.Legacy ? LegacyGames : CurrentGames;

    /// <summary>
    /// Determines whether an edition supports options introduced by current Champollion.
    /// </summary>
    /// <param name="edition">The Champollion edition.</param>
    /// <returns><see langword="true"/> when current-only options are supported; otherwise, <see langword="false"/>.</returns>
    public static bool SupportsCurrentOptions(ChampollionEdition edition) =>
        edition == ChampollionEdition.Current;

    /// <summary>
    /// Determines whether an edition and game support recreating source subdirectories.
    /// </summary>
    /// <param name="edition">The Champollion edition.</param>
    /// <param name="game">The selected game.</param>
    /// <returns><see langword="true"/> when subdirectories can be recreated; otherwise, <see langword="false"/>.</returns>
    public static bool SupportsRecreateSubdirectories(ChampollionEdition edition, SupportedGame game) =>
        edition == ChampollionEdition.Current && game == SupportedGame.Fallout4;

    /// <summary>
    /// Determines whether an operation requires an input path.
    /// </summary>
    /// <param name="operation">The Champollion operation.</param>
    /// <returns><see langword="true"/> when the operation requires input; otherwise, <see langword="false"/>.</returns>
    public static bool RequiresInput(ChampollionOperation operation) =>
        operation is ChampollionOperation.Decompile
            or ChampollionOperation.PrintInformation
            or ChampollionOperation.PrintCompileTime;

    /// <summary>
    /// Validates edition, game, operation, and option compatibility for a request.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <returns>The compatibility error messages; the list is empty when the request is compatible.</returns>
    public static IReadOnlyList<string> Validate(ChampollionRequest request)
    {
        List<string> errors = [];

        if (!GamesFor(request.Edition).Contains(request.Game))
        {
            errors.Add($"{request.Game} is not supported by {request.Edition} Champollion.");
        }

        if (request.Options.RecreateSubdirectories &&
            !SupportsRecreateSubdirectories(request.Edition, request.Game))
        {
            errors.Add("Recreate subdirectories is available only for Fallout 4 with Current Champollion.");
        }

        if (request.Options.NoDumpTree && !request.Options.Trace)
        {
            errors.Add("Do not dump trees requires tracing to be enabled.");
        }

        if (request.Edition == ChampollionEdition.Legacy && HasCurrentOnlyOption(request.Options))
        {
            errors.Add("One or more selected parameters require Current Champollion.");
        }

        return errors;
    }

    /// <summary>
    /// Determines whether decompilation options contain a current-only selection.
    /// </summary>
    /// <param name="options">The options to inspect.</param>
    /// <returns><see langword="true"/> when any current-only option is selected; otherwise, <see langword="false"/>.</returns>
    private static bool HasCurrentOnlyOption(DecompilationOptions options) =>
        options.Recursive || options.RecreateSubdirectories || options.WriteHeader || options.Trace ||
        options.NoDumpTree || options.DebugFunctions || options.NoDebugLineNumbers || options.Verbose;

    #endregion
}