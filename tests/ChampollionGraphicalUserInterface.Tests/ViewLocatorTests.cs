using ChampollionGraphicalUserInterface.ViewModels;

namespace ChampollionGraphicalUserInterface.Tests;

public sealed class ViewLocatorTests
{
    [Fact]
    public void Matches_view_models_and_rejects_other_objects()
    {
        ViewLocator locator = new();

        Assert.True(locator.Match(new TestViewModel()));
        Assert.False(locator.Match(new object()));
        Assert.False(locator.Match(null));
    }

    [Fact]
    public void Build_returns_null_for_a_null_model()
    {
        Assert.Null(new ViewLocator().Build(null));
    }

    private sealed class TestViewModel : ViewModelBase;
}