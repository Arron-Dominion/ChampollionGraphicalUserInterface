using Avalonia;
using System;

namespace ChampollionGraphicalUserInterface;

/// <summary>
/// Provides the application entry point and Avalonia configuration.
/// </summary>
sealed class Program
{
    #region Methods

    /// <summary>
    /// Starts the application with a classic desktop lifetime.
    /// </summary>
    /// <param name="args">The command-line arguments supplied to the application.</param>
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    /// <summary>
    /// Creates the Avalonia application builder used at runtime and by the visual designer.
    /// </summary>
    /// <returns>A configured Avalonia application builder.</returns>
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();

    #endregion
}
