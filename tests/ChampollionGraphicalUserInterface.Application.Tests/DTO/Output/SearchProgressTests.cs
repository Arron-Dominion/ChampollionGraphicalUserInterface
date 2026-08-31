using ChampollionGraphicalUserInterface.Application.DTO.Output;

namespace ChampollionGraphicalUserInterface.Application.Tests.DTO.Output;

public sealed class SearchProgressTests
{
    [Fact]
    public void Constructor_preserves_search_metrics()
    {
        SearchProgress progress = new(100, 4, 8);

        Assert.Equal(100, progress.DirectoriesSearched);
        Assert.Equal(4, progress.ActiveWorkers);
        Assert.Equal(8, progress.WorkerCount);
    }
}