using ChampollionGraphicalUserInterface.Application.DTO.Input;
using ChampollionGraphicalUserInterface.Application.Settings;
using ChampollionGraphicalUserInterface.Domain;

namespace ChampollionGraphicalUserInterface.Application.Tests.Settings;

public sealed class AppSettingsStoreTests
{
    [Fact]
    public void Options_are_isolated_by_edition_and_game_combination()
    {
        AppSettingsStore store = new();
        AppSettings settings = new();
        store.SetOptions(settings, ChampollionEdition.Legacy, SupportedGame.Skyrim,
            new SavedOptions { GenerateAssembly = true });
        store.SetOptions(settings, ChampollionEdition.Current, SupportedGame.Skyrim,
            new SavedOptions { Verbose = true });

        Assert.True(store.GetOptions(settings, ChampollionEdition.Legacy, SupportedGame.Skyrim)!.GenerateAssembly);
        Assert.False(store.GetOptions(settings, ChampollionEdition.Legacy, SupportedGame.Skyrim)!.Verbose);
        Assert.True(store.GetOptions(settings, ChampollionEdition.Current, SupportedGame.Skyrim)!.Verbose);
        Assert.False(store.GetOptions(settings, ChampollionEdition.Current, SupportedGame.Skyrim)!.GenerateAssembly);
        Assert.Equal(2, settings.EditionGameOptions.Count);
    }

    [Fact]
    public void Settings_path_is_in_user_data_beside_the_executable()
    {
        string applicationDirectory = Path.Combine(Path.GetTempPath(), $"ChampollionApp-{Guid.NewGuid():N}");
        AppSettingsStore store = new(applicationDirectory);

        Assert.Equal(Path.Combine(applicationDirectory, "UserData", "settings.json"), store.SettingsPath);
        Assert.Equal(Path.Combine(applicationDirectory, "UserData"), store.SettingsDirectory);
    }

    [Fact]
    public async Task Load_creates_user_data_directory_before_first_save()
    {
        string applicationDirectory = Path.Combine(Path.GetTempPath(), $"ChampollionApp-{Guid.NewGuid():N}");
        try
        {
            AppSettingsStore store = new(applicationDirectory, Path.Combine(applicationDirectory, "NoLegacyData"));

            await store.LoadAsync();

            Assert.True(Directory.Exists(store.SettingsDirectory));
            Assert.False(File.Exists(store.SettingsPath));
        }
        finally
        {
            if (Directory.Exists(applicationDirectory)) Directory.Delete(applicationDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task Load_migrates_legacy_settings_and_logs_to_executable_directory()
    {
        string root = Path.Combine(Path.GetTempPath(), $"ChampollionMigration-{Guid.NewGuid():N}");
        string applicationDirectory = Path.Combine(root, "Application");
        string legacyDirectory = Path.Combine(root, "LegacyLocalAppData");
        string legacyLogDirectory = Path.Combine(legacyDirectory, "Logs");
        Directory.CreateDirectory(legacyLogDirectory);
        await File.WriteAllTextAsync(Path.Combine(legacyDirectory, "settings.json"),
            "{\"LegacyExecutablePath\":\"C:\\\\Tools\\\\Champollion.exe\"}");
        await File.WriteAllTextAsync(Path.Combine(legacyLogDirectory, "old.log"), "old diagnostic");

        try
        {
            AppSettingsStore store = new(applicationDirectory, legacyDirectory);

            AppSettings settings = await store.LoadAsync();

            Assert.Equal(@"C:\Tools\Champollion.exe", settings.LegacyExecutablePath);
            Assert.True(File.Exists(Path.Combine(applicationDirectory, "UserData", "settings.json")));
            Assert.True(File.Exists(Path.Combine(applicationDirectory, "UserData", "Logs", "old.log")));
            Assert.False(Directory.Exists(legacyDirectory));
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task Load_preserves_malformed_settings_and_returns_defaults()
    {
        string applicationDirectory = Path.Combine(Path.GetTempPath(), $"ChampollionCorruptSettings-{Guid.NewGuid():N}");
        AppSettingsStore store = new(applicationDirectory, Path.Combine(applicationDirectory, "NoLegacyData"));
        Directory.CreateDirectory(store.SettingsDirectory);
        await File.WriteAllTextAsync(store.SettingsPath, "{\"LegacyExecutablePath\":");

        try
        {
            AppSettings settings = await store.LoadAsync();

            Assert.Null(settings.LegacyExecutablePath);
            Assert.False(File.Exists(store.SettingsPath));
            string backupPath = Assert.Single(Directory.GetFiles(store.SettingsDirectory, "settings.corrupt-*.json"));
            Assert.Equal("{\"LegacyExecutablePath\":", await File.ReadAllTextAsync(backupPath));
        }
        finally
        {
            if (Directory.Exists(applicationDirectory)) Directory.Delete(applicationDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task Save_replaces_settings_without_leaving_temporary_files()
    {
        string applicationDirectory = Path.Combine(Path.GetTempPath(), $"ChampollionAtomicSettings-{Guid.NewGuid():N}");
        AppSettingsStore store = new(applicationDirectory, Path.Combine(applicationDirectory, "NoLegacyData"));

        try
        {
            await store.SaveAsync(new AppSettings { LegacyExecutablePath = "first.exe" });
            await store.SaveAsync(new AppSettings { LegacyExecutablePath = "second.exe" });

            AppSettings settings = await store.LoadAsync();
            Assert.Equal("second.exe", settings.LegacyExecutablePath);
            Assert.Empty(Directory.GetFiles(store.SettingsDirectory, "*.tmp"));
        }
        finally
        {
            if (Directory.Exists(applicationDirectory)) Directory.Delete(applicationDirectory, recursive: true);
        }
    }
}