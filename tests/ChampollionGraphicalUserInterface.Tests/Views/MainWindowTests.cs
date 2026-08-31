using Avalonia.Controls;
using ChampollionGraphicalUserInterface.Views;

namespace ChampollionGraphicalUserInterface.Tests.Views;

public sealed class MainWindowTests
{
    [Fact]
    public void Main_window_is_an_avalonia_window()
    {
        Assert.True(typeof(Window).IsAssignableFrom(typeof(MainWindow)));
    }
}