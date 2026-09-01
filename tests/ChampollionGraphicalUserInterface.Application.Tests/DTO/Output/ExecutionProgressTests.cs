using ChampollionGraphicalUserInterface.Application.DTO.Output;

namespace ChampollionGraphicalUserInterface.Application.Tests.DTO.Output;

public sealed class ExecutionProgressTests
{
    [Fact]
    public void Constructor_preserves_progress_values()
    {
        ExecutionProgress progress = new(2, 5, "script.pex");

        Assert.Equal(2, progress.Completed);
        Assert.Equal(5, progress.Total);
        Assert.Equal("script.pex", progress.CurrentInput);
    }
}