using ChampollionGraphicalUserInterface.Application.Validation;
using ChampollionGraphicalUserInterface.Domain;

namespace ChampollionGraphicalUserInterface.Application.Tests.Validation;

public sealed class CompatibilityRulesTests
{
    [Fact]
    public void Legacy_games_are_limited_to_skyrim_releases()
    {
        IReadOnlyList<SupportedGame> games = CompatibilityRules.GamesFor(ChampollionEdition.Legacy);

        Assert.Equal([SupportedGame.Skyrim, SupportedGame.SkyrimSpecialEdition], games);
    }

    [Fact]
    public void Recreate_subdirectories_requires_current_fallout_4()
    {
        Assert.True(CompatibilityRules.SupportsRecreateSubdirectories(ChampollionEdition.Current, SupportedGame.Fallout4));
        Assert.False(CompatibilityRules.SupportsRecreateSubdirectories(ChampollionEdition.Current, SupportedGame.Starfield));
        Assert.False(CompatibilityRules.SupportsRecreateSubdirectories(ChampollionEdition.Legacy, SupportedGame.Fallout4));
    }

    [Fact]
    public void No_dump_tree_requires_trace()
    {
        ChampollionRequest request = new(ChampollionEdition.Current, SupportedGame.Starfield,
            ChampollionOperation.Decompile, "Champollion.exe", "script.pex",
            new DecompilationOptions { NoDumpTree = true });

        Assert.Contains(CompatibilityRules.Validate(request), error => error.Contains("requires tracing"));
    }
}