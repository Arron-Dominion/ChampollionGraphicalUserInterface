using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ChampollionGraphicalUserInterface.ViewModels;
using System.ComponentModel;
using System.Diagnostics;

namespace ChampollionGraphicalUserInterface.Views;

/// <summary>
/// Provides the main desktop window and handles view-specific user interactions.
/// </summary>
public partial class MainWindow : Window
{
    #region Variables

    /// <summary>The download page for the legacy Champollion edition.</summary>
    private static readonly Uri LegacyDownloadUri = new("https://www.nexusmods.com/skyrim/mods/35307");
    /// <summary>The download page for the current Champollion edition.</summary>
    private static readonly Uri CurrentDownloadUri = new("https://www.nexusmods.com/starfield/mods/4528");
    /// <summary>The view model currently subscribed for property-change notifications.</summary>
    private MainViewModel? subscribedViewModel;

    #endregion

    #region Properties

    /// <summary>Gets the view model assigned to the window.</summary>
    /// <value>The current main view model.</value>
    private MainViewModel ViewModel => (MainViewModel)DataContext!;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    /// <param name="e">The event data for the data-context change.</param>
    protected override void OnDataContextChanged(EventArgs e)
    {
        if (subscribedViewModel is not null)
        {
            subscribedViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        base.OnDataContextChanged(e);
        subscribedViewModel = DataContext as MainViewModel;
        if (subscribedViewModel is not null)
        {
            subscribedViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }
    }

    /// <summary>
    /// Scrolls the output view after the view model's output text changes.
    /// </summary>
    /// <param name="sender">The view model that raised the property-change event.</param>
    /// <param name="e">The property-change event data identifying the changed property.</param>
    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.OutputText))
        {
            Dispatcher.UIThread.Post(OutputScrollViewer.ScrollToEnd, DispatcherPriority.Background);
        }
    }

    /// <summary>
    /// Opens a file picker and assigns the selected Champollion executable path.
    /// </summary>
    /// <param name="sender">The control that raised the click event.</param>
    /// <param name="e">The routed click event data.</param>
    private async void BrowseExecutable_Click(object? sender, RoutedEventArgs e)
    {
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Champollion executable",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("Windows executable") { Patterns = ["*.exe"] }],
        });
        if (files.Count > 0) ViewModel.ExecutablePath = files[0].Path.LocalPath;
    }

    /// <summary>
    /// Opens a file picker and assigns the selected PEX input path.
    /// </summary>
    /// <param name="sender">The control that raised the click event.</param>
    /// <param name="e">The routed click event data.</param>
    private async void BrowseInputFile_Click(object? sender, RoutedEventArgs e)
    {
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select a PEX file",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("Papyrus executable") { Patterns = ["*.pex"] }],
        });
        if (files.Count > 0) ViewModel.InputPath = files[0].Path.LocalPath;
    }

    /// <summary>
    /// Opens a folder picker and assigns the selected input folder path.
    /// </summary>
    /// <param name="sender">The control that raised the click event.</param>
    /// <param name="e">The routed click event data.</param>
    private async void BrowseInputFolder_Click(object? sender, RoutedEventArgs e)
    {
        string? path = await PickFolderAsync("Select a folder containing PEX files");
        if (path is not null) ViewModel.InputPath = path;
    }

    /// <summary>
    /// Opens a folder picker and assigns the selected source output path.
    /// </summary>
    /// <param name="sender">The control that raised the click event.</param>
    /// <param name="e">The routed click event data.</param>
    private async void BrowseSourceOutput_Click(object? sender, RoutedEventArgs e)
    {
        string? path = await PickFolderAsync("Select the Papyrus source output folder");
        if (path is not null) ViewModel.SourceOutputPath = path;
    }

    /// <summary>
    /// Opens a folder picker and assigns the selected assembly output path.
    /// </summary>
    /// <param name="sender">The control that raised the click event.</param>
    /// <param name="e">The routed click event data.</param>
    private async void BrowseAssemblyOutput_Click(object? sender, RoutedEventArgs e)
    {
        string? path = await PickFolderAsync("Select the assembly output folder");
        if (path is not null) ViewModel.AssemblyOutputPath = path;
    }

    /// <summary>
    /// Opens the effective Papyrus source output directory in File Explorer.
    /// </summary>
    /// <param name="sender">The control that raised the click event.</param>
    /// <param name="e">The routed click event data.</param>
    private void OpenSourceOutput_Click(object? sender, RoutedEventArgs e) =>
        OpenOutputDirectory(ViewModel.SourceOutputPath, "Papyrus source");

    /// <summary>
    /// Opens the effective assembly output directory in File Explorer.
    /// </summary>
    /// <param name="sender">The control that raised the click event.</param>
    /// <param name="e">The routed click event data.</param>
    private void OpenAssemblyOutput_Click(object? sender, RoutedEventArgs e) =>
        OpenOutputDirectory(ViewModel.AssemblyOutputPath, "assembly");

    /// <summary>
    /// Opens a configured or input-adjacent output directory in File Explorer.
    /// </summary>
    /// <param name="configuredOutputPath">The optional output path configured by the user.</param>
    /// <param name="outputName">The output name used in status messages.</param>
    private void OpenOutputDirectory(string configuredOutputPath, string outputName)
    {
        string? outputDirectory = ViewModel.ResolveOutputDirectory(configuredOutputPath);
        if (outputDirectory is null || !Directory.Exists(outputDirectory))
        {
            ViewModel.Status = $"The {outputName} output folder does not exist yet. Run Champollion first.";
            return;
        }

        ProcessStartInfo startInfo = new("explorer.exe") { UseShellExecute = true };
        startInfo.ArgumentList.Add(outputDirectory);
        Process.Start(startInfo);
    }

    /// <summary>
    /// Selects the Champollion executable in the application directory.
    /// </summary>
    /// <param name="sender">The control that raised the click event.</param>
    /// <param name="e">The routed click event data.</param>
    private void UseApplicationDirectory_Click(object? sender, RoutedEventArgs e) => ViewModel.UseApplicationDirectory();

    /// <summary>
    /// Copies the current Champollion output to the clipboard.
    /// </summary>
    /// <param name="sender">The control that raised the click event.</param>
    /// <param name="e">The routed click event data.</param>
    private async void CopyOutput_Click(object? sender, RoutedEventArgs e)
    {
        if (Clipboard is not null) await Clipboard.SetValueAsync(DataFormat.Text, ViewModel.OutputText);
    }

    /// <summary>
    /// Navigates the embedded browser to the legacy edition download page.
    /// </summary>
    /// <param name="sender">The control that raised the click event.</param>
    /// <param name="e">The routed click event data.</param>
    private void OpenLegacyDownload_Click(object? sender, RoutedEventArgs e) => DownloadBrowser.Navigate(LegacyDownloadUri);

    /// <summary>
    /// Navigates the embedded browser to the current edition download page.
    /// </summary>
    /// <param name="sender">The control that raised the click event.</param>
    /// <param name="e">The routed click event data.</param>
    private void OpenCurrentDownload_Click(object? sender, RoutedEventArgs e) => DownloadBrowser.Navigate(CurrentDownloadUri);

    /// <summary>
    /// Navigates the embedded browser back one page.
    /// </summary>
    /// <param name="sender">The control that raised the click event.</param>
    /// <param name="e">The routed click event data.</param>
    private void BrowserBack_Click(object? sender, RoutedEventArgs e) => DownloadBrowser.GoBack();

    /// <summary>
    /// Navigates the embedded browser forward one page.
    /// </summary>
    /// <param name="sender">The control that raised the click event.</param>
    /// <param name="e">The routed click event data.</param>
    private void BrowserForward_Click(object? sender, RoutedEventArgs e) => DownloadBrowser.GoForward();

    /// <summary>
    /// Refreshes the current page in the embedded browser.
    /// </summary>
    /// <param name="sender">The control that raised the click event.</param>
    /// <param name="e">The routed click event data.</param>
    private void BrowserRefresh_Click(object? sender, RoutedEventArgs e) => DownloadBrowser.Refresh();

    /// <summary>
    /// Opens the application's MIT license from the installation directory.
    /// </summary>
    /// <param name="sender">The control that raised the click event.</param>
    /// <param name="e">The routed click event data.</param>
    private void OpenApplicationLicense_Click(object? sender, RoutedEventArgs e) => OpenLegalDocument("LICENSE.txt");

    /// <summary>
    /// Opens the third-party notices from the installation directory.
    /// </summary>
    /// <param name="sender">The control that raised the click event.</param>
    /// <param name="e">The routed click event data.</param>
    private void OpenThirdPartyNotices_Click(object? sender, RoutedEventArgs e) => OpenLegalDocument("THIRD-PARTY-NOTICES.txt");

    /// <summary>
    /// Opens a packaged legal document with the operating system's associated application.
    /// </summary>
    /// <param name="fileName">The packaged legal document file name.</param>
    private void OpenLegalDocument(string fileName)
    {
        string path = Path.Combine(AppContext.BaseDirectory, fileName);
        if (!File.Exists(path))
        {
            ViewModel.Status = $"{fileName} was not found in the application directory.";
            return;
        }

        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
    }

    /// <summary>
    /// Confirms any output directories that will be created and starts Champollion.
    /// </summary>
    /// <param name="sender">The control that raised the click event.</param>
    /// <param name="e">The routed click event data.</param>
    private async void Run_Click(object? sender, RoutedEventArgs e)
    {
        List<string> newDirectories = [];
        foreach (string path in new[] { ViewModel.SourceOutputPath, ViewModel.AssemblyOutputPath })
        {
            if (!string.IsNullOrWhiteSpace(path) && !Directory.Exists(Environment.ExpandEnvironmentVariables(path)))
                newDirectories.Add(path);
        }

        string detail = newDirectories.Count == 0
            ? "Champollion will process the selected input. Continue?"
            : $"Champollion will run and create these output directories:\n\n{string.Join("\n", newDirectories)}";
        if (await ConfirmAsync("Confirm run", detail)) await ViewModel.RunAsync();
    }

    /// <summary>
    /// Opens a folder picker with the specified title.
    /// </summary>
    /// <param name="title">The title displayed by the folder picker.</param>
    /// <returns>A task whose result is the selected local folder path, or <see langword="null"/> when no folder is selected.</returns>
    private async Task<string?> PickFolderAsync(string title)
    {
        IReadOnlyList<IStorageFolder> folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
        });
        return folders.Count > 0 ? folders[0].Path.LocalPath : null;
    }

    /// <summary>
    /// Displays a modal confirmation dialog for a Champollion run.
    /// </summary>
    /// <param name="title">The dialog window title.</param>
    /// <param name="message">The confirmation detail displayed to the user.</param>
    /// <returns>A task whose result is <see langword="true"/> when the run is confirmed; otherwise, <see langword="false"/>.</returns>
    private async Task<bool> ConfirmAsync(string title, string message)
    {
        bool confirmed = false;
        Window dialog = new()
        {
            Title = title,
            Width = 520,
            SizeToContent = SizeToContent.Height,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Background = new SolidColorBrush(Color.Parse("#F4F6F3")),
        };
        Button cancel = new()
        {
            Content = "Cancel",
            MinWidth = 96,
            Padding = new Avalonia.Thickness(16, 9),
            Background = new SolidColorBrush(Color.Parse("#E4EAE7")),
            Foreground = new SolidColorBrush(Color.Parse("#172522")),
        };
        Button run = new()
        {
            Content = "Run Champollion",
            MinWidth = 150,
            Padding = new Avalonia.Thickness(16, 9),
            Background = new SolidColorBrush(Color.Parse("#B84C32")),
            Foreground = Brushes.White,
            FontWeight = FontWeight.SemiBold,
        };
        cancel.Click += (_, _) => dialog.Close();
        run.Click += (_, _) => { confirmed = true; dialog.Close(); };
        dialog.Content = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,Auto"),
            Children =
            {
                new Border
                {
                    Background = new SolidColorBrush(Color.Parse("#20302D")),
                    Padding = new Avalonia.Thickness(26, 22),
                    Child = new StackPanel
                    {
                        Spacing = 8,
                        Children =
                        {
                            new TextBlock
                            {
                                Text = "Ready to run Champollion?",
                                FontFamily = new FontFamily("Bahnschrift"),
                                FontSize = 21,
                                FontWeight = FontWeight.Bold,
                                Foreground = new SolidColorBrush(Color.Parse("#F7F3EA")),
                            },
                            new TextBlock
                            {
                                Text = message,
                                TextWrapping = TextWrapping.Wrap,
                                Foreground = new SolidColorBrush(Color.Parse("#D8E1DD")),
                                LineHeight = 21,
                            },
                        },
                    },
                },
                new Border
                {
                    [Grid.RowProperty] = 1,
                    Padding = new Avalonia.Thickness(26, 16),
                    Child = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Spacing = 12,
                        Children = { cancel, run },
                    },
                },
            },
        };
        await dialog.ShowDialog(this);
        return confirmed;
    }

    #endregion
}