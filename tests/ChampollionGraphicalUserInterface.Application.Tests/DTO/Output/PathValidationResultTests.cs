using ChampollionGraphicalUserInterface.Application.DTO.Output;

namespace ChampollionGraphicalUserInterface.Application.Tests.DTO.Output;

public sealed class PathValidationResultTests
{
    [Fact]
    public void Stores_valid_result_properties()
    {
        PathValidationResult result = new(true, @"C:\Input\script.pex", null);

        Assert.True(result.IsValid);
        Assert.Equal(@"C:\Input\script.pex", result.ExpandedPath);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Stores_invalid_result_properties()
    {
        PathValidationResult result = new(false, null, "Invalid path");

        Assert.False(result.IsValid);
        Assert.Null(result.ExpandedPath);
        Assert.Equal("Invalid path", result.Error);
    }
}