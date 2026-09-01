using ChampollionGraphicalUserInterface.Application.DTO.Output;

namespace ChampollionGraphicalUserInterface.Application.Tests.DTO.Output;

public sealed class FileExecutionResultTests
{
    [Fact]
    public void Stores_supplied_property_values()
    {
        FileExecutionResult result = new("script.pex", 1, "output", "error", false);

        Assert.Equal("script.pex", result.InputPath);
        Assert.Equal(1, result.ExitCode);
        Assert.Equal("output", result.StandardOutput);
        Assert.Equal("error", result.StandardError);
        Assert.False(result.Succeeded);
    }
}