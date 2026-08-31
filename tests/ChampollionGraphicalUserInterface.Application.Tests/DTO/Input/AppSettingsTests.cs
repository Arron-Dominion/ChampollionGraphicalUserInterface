using ChampollionGraphicalUserInterface.Application.DTO.Input;
using ChampollionGraphicalUserInterface.Domain;

namespace ChampollionGraphicalUserInterface.Application.Tests.DTO.Input;

public sealed class AppSettingsTests
{
    [Fact]
    public void Stores_supplied_property_values()
    {
        Dictionary<string, SavedOptions> options = new()
        {
            ["Legacy:Skyrim"] = new SavedOptions { GenerateAssembly = true },
        };
        AppSettings settings = new()
        {
            LegacyExecutablePath = "legacy.exe",
            CurrentExecutablePath = "current.exe",
            LastLegacyGame = SupportedGame.Skyrim,
            LastCurrentGame = SupportedGame.Starfield,
            EditionGameOptions = options,
        };

        Assert.Equal("legacy.exe", settings.LegacyExecutablePath);
        Assert.Equal("current.exe", settings.CurrentExecutablePath);
        Assert.Equal(SupportedGame.Skyrim, settings.LastLegacyGame);
        Assert.Equal(SupportedGame.Starfield, settings.LastCurrentGame);
        Assert.Same(options, settings.EditionGameOptions);
    }

    [Fact]
    public void Defaults_to_supported_games_and_empty_profiles()
    {
        AppSettings settings = new();

        Assert.Equal(SupportedGame.Skyrim, settings.LastLegacyGame);
        Assert.Equal(SupportedGame.SkyrimSpecialEdition, settings.LastCurrentGame);
        Assert.Empty(settings.EditionGameOptions);
    }
}