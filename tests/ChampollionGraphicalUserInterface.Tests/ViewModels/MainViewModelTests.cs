using ChampollionGraphicalUserInterface.Application.DTO.Input;
using ChampollionGraphicalUserInterface.Application.Execution;
using ChampollionGraphicalUserInterface.Application.Search;
using ChampollionGraphicalUserInterface.Application.Settings;
using ChampollionGraphicalUserInterface.Application.Validation;
using ChampollionGraphicalUserInterface.Domain;
using ChampollionGraphicalUserInterface.ViewModels;

namespace ChampollionGraphicalUserInterface.Tests.ViewModels;

public sealed class MainViewModelTests
{
    [Fact]
    public void Edition_change_tolerates_transient_null_game_selection()
    {
        MainViewModel viewModel = CreateViewModel();
        viewModel.SelectedGame = SupportedGame.Fallout4;

        viewModel.SelectedGame = null;
        viewModel.SelectedEdition = ChampollionEdition.Legacy;

        Assert.NotNull(viewModel.SelectedGame);
        Assert.Contains(viewModel.SelectedGame.Value, CompatibilityRules.GamesFor(ChampollionEdition.Legacy));
    }

    [Fact]
    public void Changing_to_legacy_replaces_an_unsupported_current_game()
    {
        MainViewModel viewModel = CreateViewModel();
        viewModel.SelectedGame = SupportedGame.Starfield;

        viewModel.SelectedEdition = ChampollionEdition.Legacy;

        Assert.Equal(SupportedGame.Skyrim, viewModel.SelectedGame);
    }

    [Fact]
    public void Edition_change_restores_each_editions_game_and_game_options()
    {
        MainViewModel viewModel = CreateViewModel();
        viewModel.SelectedGame = SupportedGame.Starfield;
        viewModel.Verbose = true;
        viewModel.GenerateComments = true;

        viewModel.SelectedEdition = ChampollionEdition.Legacy;
        viewModel.SelectedGame = SupportedGame.SkyrimSpecialEdition;
        viewModel.GenerateAssembly = true;
        viewModel.GenerateComments = false;

        viewModel.SelectedEdition = ChampollionEdition.Current;

        Assert.Equal(SupportedGame.Starfield, viewModel.SelectedGame);
        Assert.True(viewModel.Verbose);
        Assert.True(viewModel.GenerateComments);
        Assert.False(viewModel.GenerateAssembly);

        viewModel.SelectedEdition = ChampollionEdition.Legacy;

        Assert.Equal(SupportedGame.SkyrimSpecialEdition, viewModel.SelectedGame);
        Assert.True(viewModel.GenerateAssembly);
        Assert.False(viewModel.GenerateComments);
        Assert.False(viewModel.Verbose);
    }

    [Fact]
    public void Same_game_has_distinct_options_for_legacy_and_current()
    {
        MainViewModel viewModel = CreateViewModel();
        viewModel.SelectedGame = SupportedGame.Skyrim;
        viewModel.Verbose = true;

        viewModel.SelectedEdition = ChampollionEdition.Legacy;
        viewModel.SelectedGame = SupportedGame.Skyrim;
        viewModel.GenerateAssembly = true;

        viewModel.SelectedEdition = ChampollionEdition.Current;

        Assert.Equal(SupportedGame.Skyrim, viewModel.SelectedGame);
        Assert.True(viewModel.Verbose);
        Assert.False(viewModel.GenerateAssembly);
    }

    [Fact]
    public void Edition_and_game_changes_clear_transient_paths()
    {
        MainViewModel viewModel = CreateViewModel();
        SetTransientPaths(viewModel);

        viewModel.SelectedGame = SupportedGame.Starfield;

        AssertTransientPathsCleared(viewModel);
        SetTransientPaths(viewModel);

        viewModel.SelectedEdition = ChampollionEdition.Legacy;

        AssertTransientPathsCleared(viewModel);
    }

    [Fact]
    public void Resolves_configured_output_directory()
    {
        MainViewModel viewModel = CreateViewModel();
        string outputPath = Path.Combine(Path.GetTempPath(), "Champollion Output");

        Assert.Equal(Path.GetFullPath(outputPath), viewModel.ResolveOutputDirectory(outputPath));
    }

    [Fact]
    public void Resolves_default_output_beside_input_file()
    {
        MainViewModel viewModel = CreateViewModel();
        viewModel.InputPath = Path.Combine(Path.GetTempPath(), "script.pex");

        Assert.Equal(Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar), viewModel.ResolveOutputDirectory(string.Empty));
    }

    [Fact]
    public void Resolves_default_output_to_input_directory()
    {
        MainViewModel viewModel = CreateViewModel();
        viewModel.InputPath = Path.GetTempPath();

        Assert.Equal(Path.GetFullPath(Path.GetTempPath()), viewModel.ResolveOutputDirectory(string.Empty));
    }

    [Fact]
    public void Does_not_resolve_output_without_configured_path_or_input()
    {
        MainViewModel viewModel = CreateViewModel();

        Assert.Null(viewModel.ResolveOutputDirectory(string.Empty));
    }

    private static void SetTransientPaths(MainViewModel viewModel)
    {
        viewModel.InputPath = @"C:\Input\script.pex";
        viewModel.SourceOutputPath = @"C:\Output\Source";
        viewModel.AssemblyOutputPath = @"C:\Output\Assembly";
    }

    private static void AssertTransientPathsCleared(MainViewModel viewModel)
    {
        Assert.Empty(viewModel.InputPath);
        Assert.Empty(viewModel.SourceOutputPath);
        Assert.Empty(viewModel.AssemblyOutputPath);
    }

    private static MainViewModel CreateViewModel()
    {
        LocalPathValidator pathValidator = new();
        return new MainViewModel(
            pathValidator,
            new ChampollionRunner(pathValidator, new DiagnosticLogWriter()),
            new ExecutableSearchService(pathValidator),
            new AppSettingsStore());
    }
}