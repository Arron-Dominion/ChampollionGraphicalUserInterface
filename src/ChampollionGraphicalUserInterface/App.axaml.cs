using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ChampollionGraphicalUserInterface.Application.Execution;
using ChampollionGraphicalUserInterface.Application.Paths;
using ChampollionGraphicalUserInterface.Application.Search;
using ChampollionGraphicalUserInterface.Application.Settings;
using ChampollionGraphicalUserInterface.Application.Validation;
using ChampollionGraphicalUserInterface.ViewModels;
using ChampollionGraphicalUserInterface.Views;

namespace ChampollionGraphicalUserInterface;

/// <summary>
/// Configures and initializes the Champollion graphical user interface application.
/// </summary>
public partial class App : Avalonia.Application
{
    #region Methods

    /// <inheritdoc/>
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <inheritdoc/>
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            LocalPathValidator pathValidator = new(ApplicationOutputPaths.GetDirectories());
            MainViewModel viewModel = new(
                pathValidator,
                new ChampollionRunner(pathValidator, new DiagnosticLogWriter()),
                new ExecutableSearchService(pathValidator),
                new AppSettingsStore());
            desktop.MainWindow = new MainWindow
            {
                DataContext = viewModel,
            };
            _ = viewModel.InitializeAsync();
        }

        base.OnFrameworkInitializationCompleted();
    }

    #endregion
}