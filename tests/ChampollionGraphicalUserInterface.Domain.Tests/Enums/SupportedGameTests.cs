using ChampollionGraphicalUserInterface.Domain;

namespace ChampollionGraphicalUserInterface.Domain.Tests.Enums;

public sealed class SupportedGameTests
{
    [Fact]
    public void Defines_all_supported_games()
    {
        Assert.Equal(
            [SupportedGame.Skyrim, SupportedGame.SkyrimSpecialEdition, SupportedGame.Fallout4,
             SupportedGame.Fallout76, SupportedGame.Starfield],
            Enum.GetValues<SupportedGame>());
    }
}