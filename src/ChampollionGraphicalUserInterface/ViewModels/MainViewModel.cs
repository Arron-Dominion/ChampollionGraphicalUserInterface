using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using ChampollionGraphicalUserInterface.Application.DTO.Input;
using ChampollionGraphicalUserInterface.Application.DTO.Output;
using ChampollionGraphicalUserInterface.Application.Execution;
using ChampollionGraphicalUserInterface.Application.Search;
using ChampollionGraphicalUserInterface.Application.Settings;
using ChampollionGraphicalUserInterface.Application.Validation;
using ChampollionGraphicalUserInterface.Domain;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChampollionGraphicalUserInterface.ViewModels;

/// <summary>
/// Coordinates application state, user options, executable discovery, and Champollion execution.
/// </summary>
public partial class MainViewModel : ViewModelBase
{
    #region Variables

    /// <summary>The validator used to check local executable and input paths.</summary>
    private readonly LocalPathValidator pathValidator;
    /// <summary>The runner used to execute Champollion requests.</summary>
    private readonly ChampollionRunner runner;
    /// <summary>The service used to locate Champollion executables.</summary>
    private readonly ExecutableSearchService searchService;
    /// <summary>The store used to load and save application settings.</summary>
    private readonly AppSettingsStore settingsStore;
    /// <summary>The settings currently loaded for the application.</summary>
    private AppSettings settings = new();
    /// <summary>The cancellation source for the active executable search.</summary>
    private CancellationTokenSource? searchCancellation;
    /// <summary>Indicates whether the available game collection is being refreshed.</summary>
    private bool isRefreshingGames;
    /// <summary>The input path associated with the current live-output section.</summary>
    private string? currentOutputInput;

    /// <summary>The selected Champollion edition.</summary>
    [ObservableProperty] private ChampollionEdition selectedEdition = ChampollionEdition.Current;
    /// <summary>The selected supported game.</summary>
    [ObservableProperty] private SupportedGame? selectedGame = SupportedGame.SkyrimSpecialEdition;
    /// <summary>The selected Champollion operation.</summary>
    [ObservableProperty] private ChampollionOperation selectedOperation;
    /// <summary>The path to the selected Champollion executable.</summary>
    [ObservableProperty] private string executablePath = string.Empty;
    /// <summary>The validation message for the executable path.</summary>
    [ObservableProperty] private string executableValidation = "Select Champollion.exe, use the app directory, or start automatic search.";
    /// <summary>The selected input file or directory path.</summary>
    [ObservableProperty] private string inputPath = string.Empty;
    /// <summary>The validation message for the input path.</summary>
    [ObservableProperty] private string inputValidation = "Select one .pex file or a folder containing .pex files.";
    /// <summary>The selected Papyrus source output path.</summary>
    [ObservableProperty] private string sourceOutputPath = string.Empty;
    /// <summary>The selected assembly output path.</summary>
    [ObservableProperty] private string assemblyOutputPath = string.Empty;
    /// <summary>Indicates whether assembly output should be generated.</summary>
    [ObservableProperty] private bool generateAssembly;
    /// <summary>Indicates whether comments should be generated.</summary>
    [ObservableProperty] private bool generateComments;
    /// <summary>Indicates whether input directories should be processed recursively.</summary>
    [ObservableProperty] private bool recursive;
    /// <summary>Indicates whether input subdirectories should be recreated in output.</summary>
    [ObservableProperty] private bool recreateSubdirectories;
    /// <summary>Indicates whether source headers should be written.</summary>
    [ObservableProperty] private bool writeHeader;
    /// <summary>Indicates whether trace output should be generated.</summary>
    [ObservableProperty] private bool trace;
    /// <summary>Indicates whether trace syntax-tree output should be suppressed.</summary>
    [ObservableProperty] private bool noDumpTree;
    /// <summary>Indicates whether debug functions should be emitted.</summary>
    [ObservableProperty] private bool debugFunctions;
    /// <summary>Indicates whether debug line numbers should be suppressed.</summary>
    [ObservableProperty] private bool noDebugLineNumbers;
    /// <summary>Indicates whether verbose output should be enabled.</summary>
    [ObservableProperty] private bool verbose;
    /// <summary>Indicates whether Champollion is currently running.</summary>
    [ObservableProperty] private bool isRunning;
    /// <summary>Indicates whether an executable search is currently running.</summary>
    [ObservableProperty] private bool isSearching;
    /// <summary>The number of files completed by the current run.</summary>
    [ObservableProperty] private int completedFiles;
    /// <summary>The total number of files in the current run.</summary>
    [ObservableProperty] private int totalFiles = 1;
    /// <summary>The current application status message.</summary>
    [ObservableProperty] private string status = "Ready";
    /// <summary>The accumulated Champollion output text.</summary>
    [ObservableProperty] private string outputText = "Champollion output will appear here.";
    /// <summary>The path to the diagnostic log for the most recent run.</summary>
    [ObservableProperty] private string? logPath;

    #endregion

    #region Properties

    /// <summary>Gets the available Champollion editions.</summary>
    /// <value>The supported Champollion editions.</value>
    public IReadOnlyList<ChampollionEdition> Editions { get; } = Enum.GetValues<ChampollionEdition>();

    /// <summary>Gets the available Champollion operations.</summary>
    /// <value>The supported Champollion operations.</value>
    public IReadOnlyList<ChampollionOperation> Operations { get; } = Enum.GetValues<ChampollionOperation>();

    /// <summary>Gets the games available for the selected edition.</summary>
    /// <value>An observable collection of supported games.</value>
    public ObservableCollection<SupportedGame> Games { get; } = [];

    /// <summary>Gets whether the current Champollion edition is selected.</summary>
    /// <value><see langword="true"/> for the current edition; otherwise, <see langword="false"/>.</value>
    public bool IsCurrentEdition => SelectedEdition == ChampollionEdition.Current;
    /// <summary>Gets whether the selected edition and game support recreating subdirectories.</summary>
    /// <value><see langword="true"/> when the option is supported; otherwise, <see langword="false"/>.</value>
    public bool CanRecreateSubdirectories => CompatibilityRules.SupportsRecreateSubdirectories(SelectedEdition, EffectiveGame);
    /// <summary>Gets whether the selected operation requires an input path.</summary>
    /// <value><see langword="true"/> when input is required; otherwise, <see langword="false"/>.</value>
    public bool RequiresInput => CompatibilityRules.RequiresInput(SelectedOperation);
    /// <summary>Gets whether a Champollion run can be started.</summary>
    /// <value><see langword="true"/> when no run or search is active; otherwise, <see langword="false"/>.</value>
    public bool CanRun => !IsRunning && !IsSearching;
    /// <summary>Gets whether a diagnostic log is available.</summary>
    /// <value><see langword="true"/> when a log path is available; otherwise, <see langword="false"/>.</value>
    public bool HasLog => LogPath is not null;
    /// <summary>Gets the application settings file location.</summary>
    /// <value>The full settings file path.</value>
    public string SettingsLocation => settingsStore.SettingsPath;
    /// <summary>Gets a description of the selected edition's game support.</summary>
    /// <value>A user-facing edition description.</value>
    public string EditionDescription => SelectedEdition == ChampollionEdition.Legacy
        ? "Legacy supports Skyrim Original Release and Skyrim Special Edition."
        : "Current adds Fallout 4, Fallout 76, Starfield, and modern parameters.";

    /// <summary>Gets the selected game, falling back to the first game supported by the selected edition.</summary>
    /// <value>The effective game used for settings and execution.</value>
    private SupportedGame EffectiveGame => SelectedGame ?? CompatibilityRules.GamesFor(SelectedEdition)[0];

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    /// <param name="pathValidator">The validator used to check local paths.</param>
    /// <param name="runner">The service used to execute Champollion.</param>
    /// <param name="searchService">The service used to locate Champollion executables.</param>
    /// <param name="settingsStore">The store used to persist application settings.</param>
    public MainViewModel(LocalPathValidator pathValidator, ChampollionRunner runner,
        ExecutableSearchService searchService, AppSettingsStore settingsStore)
    {
        this.pathValidator = pathValidator;
        this.runner = runner;
        this.searchService = searchService;
        this.settingsStore = settingsStore;
        RefreshGames();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Loads saved settings and applies them to the current selections.
    /// </summary>
    /// <returns>A task that represents the asynchronous initialization.</returns>
    public async Task InitializeAsync()
    {
        settings = await settingsStore.LoadAsync();
        RefreshGames(GetRememberedGame(SelectedEdition));
        ExecutablePath = GetSavedExecutable();
        ApplySavedOptions();
    }

    /// <summary>
    /// Selects the Champollion executable located in the application directory.
    /// </summary>
    public void UseApplicationDirectory() =>
        ExecutablePath = Path.Combine(AppContext.BaseDirectory, ExecutableSearchService.ExpectedExecutableFileName);

    /// <summary>
    /// Saves the current settings and runs Champollion with live progress and output updates.
    /// </summary>
    /// <returns>A task that represents the asynchronous run.</returns>
    public async Task RunAsync()
    {
        IsRunning = true;
        Status = "Running Champollion...";
        OutputText = string.Empty;
        LogPath = null;
        CompletedFiles = 0;
        TotalFiles = 1;
        currentOutputInput = null;
        try
        {
            await SaveSettingsAsync();
            Progress<ExecutionProgress> progress = new(value =>
            {
                CompletedFiles = value.Completed;
                TotalFiles = Math.Max(value.Total, 1);
                Status = $"Processed {value.Completed} of {value.Total}: {Path.GetFileName(value.CurrentInput)}";
            });
            Progress<ExecutionOutput> liveOutput = new(value =>
            {
                string input = value.InputPath ?? SelectedOperation.ToString();
                if (!string.Equals(currentOutputInput, input, StringComparison.OrdinalIgnoreCase))
                {
                    currentOutputInput = input;
                    OutputText += $"{(OutputText.Length == 0 ? string.Empty : Environment.NewLine)}=== {input} ==={Environment.NewLine}";
                }

                OutputText += value.IsError ? $"[stderr] {value.Text}" : value.Text;
            });
            ExecutionSummary summary = await runner.RunAsync(CreateRequest(), progress, liveOutput);
            AppendResultStatuses(summary);
            LogPath = summary.LogPath;
            Status = summary.FailedCount == 0
                ? $"Complete. {summary.SuccessfulCount} operation(s) succeeded."
                : $"Complete with {summary.FailedCount} failure(s). Remaining files were still attempted.";
        }
        catch (Exception exception)
        {
            OutputText = exception.Message;
            Status = "Unable to run. Correct the settings and try again.";
        }
        finally
        {
            IsRunning = false;
        }
    }

    /// <summary>
    /// Searches local fixed drives for an executable matching the selected edition.
    /// </summary>
    /// <returns>A task that represents the asynchronous search.</returns>
    [RelayCommand]
    private async Task SearchAsync()
    {
        string previousPath = ExecutablePath;
        searchCancellation = new CancellationTokenSource();
        IsSearching = true;
        Status = "Searching local fixed drives. Network and removable drives are excluded...";
        try
        {
            Progress<SearchProgress> progress = new(value =>
                Status = $"Searching with {value.WorkerCount} workers: {value.DirectoriesSearched:N0} directories checked, {value.ActiveWorkers} active.");
            string? found = await searchService.FindAsync(SelectedEdition, progress, searchCancellation.Token);
            if (found is null)
            {
                ExecutablePath = previousPath;
                Status = $"No {SelectedEdition} Champollion executable was found. The previous path was restored.";
                return;
            }

            ExecutablePath = found;
            await SaveSettingsAsync();
            Status = $"{SelectedEdition} Champollion.exe found and saved.";
        }
        catch (OperationCanceledException)
        {
            ExecutablePath = previousPath;
            Status = "Search cancelled. The previous path was restored.";
        }
        finally
        {
            IsSearching = false;
            searchCancellation.Dispose();
            searchCancellation = null;
        }
    }

    /// <summary>
    /// Cancels the active executable search, if one exists.
    /// </summary>
    [RelayCommand]
    private void CancelSearch() => searchCancellation?.Cancel();

    /// <summary>
    /// Opens File Explorer with the current diagnostic log selected.
    /// </summary>
    [RelayCommand]
    private void OpenLogFolder()
    {
        if (LogPath is not null)
            Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{LogPath}\"") { UseShellExecute = true });
    }

    /// <summary>
    /// Opens the directory containing application settings in File Explorer.
    /// </summary>
    [RelayCommand]
    private void OpenSettingsFolder() =>
        Process.Start(new ProcessStartInfo("explorer.exe", $"\"{settingsStore.SettingsDirectory}\"") { UseShellExecute = true });

    /// <summary>
    /// Resolves a configured output directory or Champollion's input-adjacent default.
    /// </summary>
    /// <param name="configuredOutputPath">The optional output directory configured by the user.</param>
    /// <returns>The expanded absolute output directory, or <see langword="null"/> when no directory can be resolved.</returns>
    public string? ResolveOutputDirectory(string configuredOutputPath)
    {
        string candidate = string.IsNullOrWhiteSpace(configuredOutputPath) ? InputPath : configuredOutputPath;
        if (string.IsNullOrWhiteSpace(candidate))
        {
            return null;
        }

        try
        {
            string expandedPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables(candidate.Trim().Trim('"')));
            return string.IsNullOrWhiteSpace(configuredOutputPath) && !Directory.Exists(expandedPath)
                ? Path.GetDirectoryName(expandedPath)
                : expandedPath;
        }
        catch (Exception exception) when (exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            return null;
        }
    }

    /// <summary>
    /// Updates edition-dependent games, paths, options, and computed properties after the selected edition changes.
    /// </summary>
    /// <param name="oldValue">The previously selected edition.</param>
    /// <param name="newValue">The newly selected edition.</param>
    partial void OnSelectedEditionChanged(ChampollionEdition oldValue, ChampollionEdition newValue)
    {
        CaptureOptions(oldValue, EffectiveGame);
        RememberGame(oldValue, EffectiveGame);
        SaveExecutable(oldValue);
        RefreshGames(GetRememberedGame(newValue));
        ExecutablePath = GetSavedExecutable();
        ClearTransientPaths();
        ClearIncompatibleOptions();
        ApplySavedOptions();
        OnPropertyChanged(nameof(IsCurrentEdition));
        OnPropertyChanged(nameof(CanRecreateSubdirectories));
        OnPropertyChanged(nameof(EditionDescription));
    }

    /// <summary>
    /// Updates game-dependent paths and options after the selected game changes.
    /// </summary>
    /// <param name="oldValue">The previously selected game.</param>
    /// <param name="newValue">The newly selected game.</param>
    partial void OnSelectedGameChanged(SupportedGame? oldValue, SupportedGame? newValue)
    {
        if (isRefreshingGames || newValue is null)
        {
            return;
        }

        if (oldValue is not null)
        {
            CaptureOptions(oldValue.Value);
        }

        RememberGame(SelectedEdition, newValue.Value);
        ClearTransientPaths();
        ClearIncompatibleOptions();
        ApplySavedOptions();
        OnPropertyChanged(nameof(CanRecreateSubdirectories));
    }

    /// <summary>Raises dependent-property notifications after the selected operation changes.</summary>
    /// <param name="value">The newly selected operation.</param>
    partial void OnSelectedOperationChanged(ChampollionOperation value) => OnPropertyChanged(nameof(RequiresInput));
    /// <summary>Raises dependent-property notifications after the running state changes.</summary>
    /// <param name="value">The new running state.</param>
    partial void OnIsRunningChanged(bool value) => OnPropertyChanged(nameof(CanRun));
    /// <summary>Raises dependent-property notifications after the searching state changes.</summary>
    /// <param name="value">The new searching state.</param>
    partial void OnIsSearchingChanged(bool value) => OnPropertyChanged(nameof(CanRun));
    /// <summary>Raises dependent-property notifications after the log path changes.</summary>
    /// <param name="value">The new diagnostic log path.</param>
    partial void OnLogPathChanged(string? value) => OnPropertyChanged(nameof(HasLog));
    /// <summary>Clears the dump-tree option when trace output is disabled.</summary>
    /// <param name="value">The new trace-enabled state.</param>
    partial void OnTraceChanged(bool value) { if (!value) NoDumpTree = false; }

    /// <summary>
    /// Validates the newly selected executable path.
    /// </summary>
    /// <param name="value">The new executable path.</param>
    partial void OnExecutablePathChanged(string value)
    {
        PathValidationResult result = pathValidator.ValidateExecutable(value);
        ExecutableValidation = result.IsValid ? "Executable path is valid." : result.Error!;
    }

    /// <summary>
    /// Validates the newly selected input path.
    /// </summary>
    /// <param name="value">The new input path.</param>
    partial void OnInputPathChanged(string value)
    {
        PathValidationResult result = pathValidator.ValidateInput(value);
        InputValidation = result.IsValid ? "Input path is valid." : result.Error!;
    }

    /// <summary>
    /// Creates an execution request from the current selections and options.
    /// </summary>
    /// <returns>The request to execute.</returns>
    private ChampollionRequest CreateRequest() => new(SelectedEdition, EffectiveGame, SelectedOperation,
        ExecutablePath, RequiresInput ? InputPath : null, new DecompilationOptions
        {
            GenerateAssembly = GenerateAssembly,
            AssemblyOutputPath = GenerateAssembly && !string.IsNullOrWhiteSpace(AssemblyOutputPath) ? AssemblyOutputPath : null,
            SourceOutputPath = string.IsNullOrWhiteSpace(SourceOutputPath) ? null : SourceOutputPath,
            GenerateComments = GenerateComments,
            Recursive = Recursive,
            RecreateSubdirectories = RecreateSubdirectories,
            WriteHeader = WriteHeader,
            Trace = Trace,
            NoDumpTree = NoDumpTree,
            DebugFunctions = DebugFunctions,
            NoDebugLineNumbers = NoDebugLineNumbers,
            Verbose = Verbose,
        });

    /// <summary>
    /// Refreshes the games supported by the selected edition.
    /// </summary>
    /// <param name="preferredGame">The game to select when it remains available.</param>
    private void RefreshGames(SupportedGame? preferredGame = null)
    {
        isRefreshingGames = true;
        try
        {
            SupportedGame? previous = preferredGame ?? SelectedGame;
            Games.Clear();
            foreach (SupportedGame game in CompatibilityRules.GamesFor(SelectedEdition)) Games.Add(game);
            SelectedGame = previous is not null && Games.Contains(previous.Value) ? previous : Games[0];
        }
        finally
        {
            isRefreshingGames = false;
        }
    }

    /// <summary>
    /// Clears options that are incompatible with the current edition, game, or trace state.
    /// </summary>
    private void ClearIncompatibleOptions()
    {
        if (!IsCurrentEdition)
        {
            Recursive = RecreateSubdirectories = WriteHeader = Trace = NoDumpTree = false;
            DebugFunctions = NoDebugLineNumbers = Verbose = false;
        }
        if (!CanRecreateSubdirectories) RecreateSubdirectories = false;
        if (!Trace) NoDumpTree = false;
    }

    /// <summary>
    /// Applies saved options for the selected edition and effective game.
    /// </summary>
    private void ApplySavedOptions()
    {
        ResetOptionSelections();
        SavedOptions? saved = settingsStore.GetOptions(settings, SelectedEdition, EffectiveGame);
        if (saved is null) return;
        GenerateAssembly = saved.GenerateAssembly;
        GenerateComments = saved.GenerateComments;
        Recursive = IsCurrentEdition && saved.Recursive;
        RecreateSubdirectories = CanRecreateSubdirectories && saved.RecreateSubdirectories;
        WriteHeader = IsCurrentEdition && saved.WriteHeader;
        Trace = IsCurrentEdition && saved.Trace;
        NoDumpTree = Trace && saved.NoDumpTree;
        DebugFunctions = IsCurrentEdition && saved.DebugFunctions;
        NoDebugLineNumbers = IsCurrentEdition && saved.NoDebugLineNumbers;
        Verbose = IsCurrentEdition && saved.Verbose;
    }

    /// <summary>
    /// Captures the current selections and saves application settings.
    /// </summary>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    private async Task SaveSettingsAsync()
    {
        SaveCurrentExecutable();
        CaptureOptions(EffectiveGame);
        RememberGame(SelectedEdition, EffectiveGame);
        await settingsStore.SaveAsync(settings);
    }

    /// <summary>
    /// Captures the current options for the selected edition and specified game.
    /// </summary>
    /// <param name="game">The game associated with the option values.</param>
    private void CaptureOptions(SupportedGame game)
    {
        CaptureOptions(SelectedEdition, game);
    }

    /// <summary>
    /// Captures the current options for the specified edition and game.
    /// </summary>
    /// <param name="edition">The edition associated with the option values.</param>
    /// <param name="game">The game associated with the option values.</param>
    private void CaptureOptions(ChampollionEdition edition, SupportedGame game)
    {
        settingsStore.SetOptions(settings, edition, game, new SavedOptions
        {
            GenerateAssembly = GenerateAssembly, GenerateComments = GenerateComments, Recursive = Recursive,
            RecreateSubdirectories = RecreateSubdirectories, WriteHeader = WriteHeader, Trace = Trace,
            NoDumpTree = NoDumpTree, DebugFunctions = DebugFunctions,
            NoDebugLineNumbers = NoDebugLineNumbers, Verbose = Verbose,
        });
    }

    /// <summary>
    /// Resets all selectable execution options.
    /// </summary>
    private void ResetOptionSelections()
    {
        GenerateAssembly = GenerateComments = Recursive = RecreateSubdirectories = false;
        WriteHeader = Trace = NoDumpTree = DebugFunctions = NoDebugLineNumbers = Verbose = false;
    }

    /// <summary>
    /// Clears input and output paths that should not carry across edition or game changes.
    /// </summary>
    private void ClearTransientPaths()
    {
        InputPath = string.Empty;
        SourceOutputPath = string.Empty;
        AssemblyOutputPath = string.Empty;
    }

    /// <summary>
    /// Appends per-file execution statuses to the displayed output.
    /// </summary>
    /// <param name="summary">The execution summary containing the file results.</param>
    private void AppendResultStatuses(ExecutionSummary summary)
    {
        StringBuilder statuses = new();
        statuses.AppendLine();
        statuses.AppendLine("=== Summary ===");
        foreach (FileExecutionResult result in summary.Results)
        {
            statuses.AppendLine($"[{(result.Succeeded ? "OK" : "FAILED")}] {result.InputPath ?? SelectedOperation.ToString()} (exit code {result.ExitCode})");
        }

        OutputText += statuses.ToString();
    }

    /// <summary>
    /// Remembers the selected game for the specified edition.
    /// </summary>
    /// <param name="edition">The edition whose game selection should be remembered.</param>
    /// <param name="game">The selected game.</param>
    private void RememberGame(ChampollionEdition edition, SupportedGame game) => settings = edition == ChampollionEdition.Legacy
        ? settings with { LastLegacyGame = game }
        : settings with { LastCurrentGame = game };

    /// <summary>
    /// Gets the remembered game for the specified edition.
    /// </summary>
    /// <param name="edition">The edition whose remembered game should be returned.</param>
    /// <returns>The remembered game.</returns>
    private SupportedGame GetRememberedGame(ChampollionEdition edition) => edition == ChampollionEdition.Legacy
        ? settings.LastLegacyGame
        : settings.LastCurrentGame;

    /// <summary>
    /// Saves the current executable path for the selected edition.
    /// </summary>
    private void SaveCurrentExecutable() => SaveExecutable(SelectedEdition);

    /// <summary>
    /// Saves the current executable path for the specified edition.
    /// </summary>
    /// <param name="edition">The edition whose executable path should be saved.</param>
    private void SaveExecutable(ChampollionEdition edition) => settings = edition == ChampollionEdition.Legacy
        ? settings with { LegacyExecutablePath = ExecutablePath }
        : settings with { CurrentExecutablePath = ExecutablePath };

    /// <summary>
    /// Gets the saved executable path for the selected edition.
    /// </summary>
    /// <returns>The saved executable path, or an empty string when none is saved.</returns>
    private string GetSavedExecutable() => SelectedEdition == ChampollionEdition.Legacy
        ? settings.LegacyExecutablePath ?? string.Empty : settings.CurrentExecutablePath ?? string.Empty;

    #endregion
}
