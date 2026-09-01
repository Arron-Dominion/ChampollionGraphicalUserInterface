using ChampollionGraphicalUserInterface.Application.DTO.Output;

namespace ChampollionGraphicalUserInterface.Application.Tests.DTO.Output;

public sealed class ExecutionSummaryTests
{
    [Fact]
    public void Stores_supplied_property_values()
    {
        FileExecutionResult[] results =
            [new FileExecutionResult("success.pex", 0, "", "", true),
             new FileExecutionResult("failure.pex", 1, "", "error", false)];
        ExecutionSummary summary = new(
            results,
            "diagnostic.log",
            1,
            1);

        Assert.Same(results, summary.Results);
        Assert.Equal(1, summary.SuccessfulCount);
        Assert.Equal(1, summary.FailedCount);
        Assert.Equal("diagnostic.log", summary.LogPath);
    }
}