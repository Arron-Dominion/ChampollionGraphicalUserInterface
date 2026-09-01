using ChampollionGraphicalUserInterface.Domain;

namespace ChampollionGraphicalUserInterface.Domain.Tests.Enums;

public sealed class ChampollionEditionTests
{
    [Fact]
    public void Defines_all_editions()
    {
        Assert.Equal(
            [ChampollionEdition.Legacy, ChampollionEdition.Current],
            Enum.GetValues<ChampollionEdition>());
    }
}