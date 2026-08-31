using ChampollionGraphicalUserInterface.Application.Enums;

namespace ChampollionGraphicalUserInterface.Application.Tests.Enums;

public sealed class ExecutableClassificationTests
{
    [Fact]
    public void Defines_all_classification_states()
    {
        Assert.Equal(
            [ExecutableClassification.Unknown, ExecutableClassification.Legacy, ExecutableClassification.Current],
            Enum.GetValues<ExecutableClassification>());
    }
}