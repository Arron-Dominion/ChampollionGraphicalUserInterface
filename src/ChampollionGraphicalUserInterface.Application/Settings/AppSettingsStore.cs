using System.Text.Json;
using ChampollionGraphicalUserInterface.Application.DTO.Input;
using ChampollionGraphicalUserInterface.Domain;

namespace ChampollionGraphicalUserInterface.Application.Settings;

/// <summary>
/// Loads, saves, and migrates application settings and option profiles.
/// </summary>
public sealed class AppSettingsStore
{
    #region Variables

    /// <summary>
    /// The current user data directory.
    /// </summary>
    private readonly string dataDirectory;

    /// <summary>
    /// The current settings file path.
    /// </summary>
    private readonly string settingsPath;

    /// <summary>
    /// The former user data directory checked during migration.
    /// </summary>
    private readonly string legacyDataDirectory;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the path to the current settings file.
    /// </summary>
    /// <value>The absolute settings file path.</value>
    public string SettingsPath => settingsPath;

    /// <summary>
    /// Gets the directory containing current application settings and logs.
    /// </summary>
    /// <value>The absolute user data directory path.</value>
    public string SettingsDirectory => dataDirectory;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AppSettingsStore"/> class.
    /// </summary>
    /// <param name="applicationDirectory">The application directory used as the root for current user data.</param>
    /// <param name="legacyDataDirectory">The former user data directory to migrate, if present.</param>
    public AppSettingsStore(string? applicationDirectory = null, string? legacyDataDirectory = null)
    {
        dataDirectory = Path.Combine(applicationDirectory ?? AppContext.BaseDirectory, "UserData");
        settingsPath = Path.Combine(dataDirectory, "settings.json");
        this.legacyDataDirectory = legacyDataDirectory ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ChampollionGraphicalUserInterface");
    }

    #endregion

    #region Methods

    /// <summary>
    /// Migrates legacy data and loads the current application settings.
    /// </summary>
    /// <param name="cancellationToken">The token used to cancel settings deserialization.</param>
    /// <returns>A task containing the saved settings, or default settings when no valid file exists.</returns>
    public async Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default)
    {
        MigrateLegacyData();
        Directory.CreateDirectory(dataDirectory);
        if (!File.Exists(settingsPath))
        {
            return new AppSettings();
        }

        try
        {
            await using FileStream stream = File.OpenRead(settingsPath);
            return await JsonSerializer.DeserializeAsync<AppSettings>(stream, cancellationToken: cancellationToken)
                ?? new AppSettings();
        }
        catch (JsonException)
        {
            PreserveInvalidSettings();
            return new AppSettings();
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            return new AppSettings();
        }
    }

    /// <summary>
    /// Saves application settings to the current settings file.
    /// </summary>
    /// <param name="settings">The settings to save.</param>
    /// <param name="cancellationToken">The token used to cancel settings serialization.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    public async Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(dataDirectory);
        string temporaryPath = Path.Combine(dataDirectory, $"settings.{Guid.NewGuid():N}.tmp");
        try
        {
            await using (FileStream stream = File.Create(temporaryPath))
            {
                await JsonSerializer.SerializeAsync(
                    stream,
                    settings,
                    new JsonSerializerOptions { WriteIndented = true },
                    cancellationToken);
            }

            File.Move(temporaryPath, settingsPath, overwrite: true);
        }
        finally
        {
            File.Delete(temporaryPath);
        }
    }

    /// <summary>
    /// Moves malformed settings aside for troubleshooting without blocking application startup.
    /// </summary>
    private void PreserveInvalidSettings()
    {
        string backupPath = Path.Combine(
            dataDirectory,
            $"settings.corrupt-{DateTimeOffset.UtcNow:yyyyMMddTHHmmssfffZ}-{Guid.NewGuid():N}.json");
        try
        {
            File.Move(settingsPath, backupPath);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
        }
    }

    /// <summary>
    /// Gets saved options for an edition and game profile.
    /// </summary>
    /// <param name="settings">The application settings containing option profiles.</param>
    /// <param name="edition">The Champollion edition for the profile.</param>
    /// <param name="game">The supported game for the profile.</param>
    /// <returns>The saved options, or <see langword="null"/> when the profile has not been saved.</returns>
    public SavedOptions? GetOptions(AppSettings settings, ChampollionEdition edition, SupportedGame game) =>
        settings.EditionGameOptions.GetValueOrDefault(ProfileKey(edition, game));

    /// <summary>
    /// Stores options for an edition and game profile.
    /// </summary>
    /// <param name="settings">The application settings to update.</param>
    /// <param name="edition">The Champollion edition for the profile.</param>
    /// <param name="game">The supported game for the profile.</param>
    /// <param name="options">The options to store.</param>
    public void SetOptions(
        AppSettings settings,
        ChampollionEdition edition,
        SupportedGame game,
        SavedOptions options) => settings.EditionGameOptions[ProfileKey(edition, game)] = options;

    /// <summary>
    /// Moves settings and log files from the former user data directory when present.
    /// </summary>
    private void MigrateLegacyData()
    {
        string legacySettingsPath = Path.Combine(legacyDataDirectory, "settings.json");
        CopyThenDelete(legacySettingsPath, settingsPath);

        string legacyLogDirectory = Path.Combine(legacyDataDirectory, "Logs");
        if (Directory.Exists(legacyLogDirectory))
        {
            string newLogDirectory = Path.Combine(dataDirectory, "Logs");
            foreach (string legacyLogPath in Directory.EnumerateFiles(legacyLogDirectory, "*.log"))
            {
                CopyThenDelete(legacyLogPath, Path.Combine(newLogDirectory, Path.GetFileName(legacyLogPath)));
            }

            DeleteIfEmpty(legacyLogDirectory);
        }

        DeleteIfEmpty(legacyDataDirectory);
    }

    /// <summary>
    /// Copies a legacy file when necessary and removes the source file.
    /// </summary>
    /// <param name="sourcePath">The legacy source file path.</param>
    /// <param name="destinationPath">The current destination file path.</param>
    private static void CopyThenDelete(string sourcePath, string destinationPath)
    {
        if (!File.Exists(sourcePath))
        {
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
        if (!File.Exists(destinationPath))
        {
            File.Copy(sourcePath, destinationPath);
        }

        File.Delete(sourcePath);
    }

    /// <summary>
    /// Deletes an existing directory when it contains no file system entries.
    /// </summary>
    /// <param name="directory">The directory to delete when empty.</param>
    private static void DeleteIfEmpty(string directory)
    {
        if (Directory.Exists(directory) && !Directory.EnumerateFileSystemEntries(directory).Any())
        {
            Directory.Delete(directory);
        }
    }

    /// <summary>
    /// Creates the storage key for an edition and game option profile.
    /// </summary>
    /// <param name="edition">The Champollion edition.</param>
    /// <param name="game">The supported game.</param>
    /// <returns>The option profile key.</returns>
    private static string ProfileKey(ChampollionEdition edition, SupportedGame game) => $"{edition}:{game}";

    #endregion
}