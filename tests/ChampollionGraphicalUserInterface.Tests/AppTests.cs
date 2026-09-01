namespace ChampollionGraphicalUserInterface.Tests;

public sealed class AppTests
{
    [Fact]
    public void App_is_an_avalonia_application()
    {
        Assert.True(typeof(Avalonia.Application).IsAssignableFrom(typeof(App)));
    }
}