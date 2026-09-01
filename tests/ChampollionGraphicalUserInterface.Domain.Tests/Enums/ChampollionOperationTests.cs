using ChampollionGraphicalUserInterface.Domain;

namespace ChampollionGraphicalUserInterface.Domain.Tests.Enums;

public sealed class ChampollionOperationTests
{
    [Fact]
    public void Defines_all_operations()
    {
        Assert.Equal(
            [ChampollionOperation.Decompile, ChampollionOperation.Help, ChampollionOperation.Version,
             ChampollionOperation.PrintInformation, ChampollionOperation.PrintCompileTime],
            Enum.GetValues<ChampollionOperation>());
    }
}