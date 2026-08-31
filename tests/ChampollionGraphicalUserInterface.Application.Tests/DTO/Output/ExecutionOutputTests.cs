using ChampollionGraphicalUserInterface.Application.DTO.Output;

namespace ChampollionGraphicalUserInterface.Application.Tests.DTO.Output;

public sealed class ExecutionOutputTests
{
    [Fact]
    public void Constructor_preserves_output_values()
    {
        ExecutionOutput output = new("script.pex", true, "failure");

        Assert.Equal("script.pex", output.InputPath);
        Assert.True(output.IsError);
        Assert.Equal("failure", output.Text);
    }
}