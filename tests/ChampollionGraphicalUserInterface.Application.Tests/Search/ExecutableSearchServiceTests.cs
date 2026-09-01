using ChampollionGraphicalUserInterface.Application.Search;
using ChampollionGraphicalUserInterface.Application.Validation;
using ChampollionGraphicalUserInterface.Domain;

namespace ChampollionGraphicalUserInterface.Application.Tests.Search;

public sealed class ExecutableSearchServiceTests
{
    [Theory]
    [InlineData(1, 4)]
    [InlineData(8, 16)]
    [InlineData(16, 32)]
    [InlineData(64, 32)]
    public void Worker_count_scales_for_io_bound_searches(int processorCount, int expectedWorkerCount)
    {
        Assert.Equal(expectedWorkerCount, ExecutableSearchService.CalculateWorkerCount(processorCount));
    }

    [Fact]
    public async Task Search_traverses_independent_roots_concurrently()
    {
        string root = Path.Combine(Path.GetTempPath(), $"ChampollionSearch-{Guid.NewGuid():N}");
        string[] roots = Enumerable.Range(0, 8).Select(index => Path.Combine(root, index.ToString())).ToArray();
        foreach (string directory in roots)
        {
            Directory.CreateDirectory(directory);
        }

        using ManualResetEventSlim workersEntered = new();
        int enteredWorkerCount = 0;
        try
        {
            ExecutableSearchService service = new(
                new LocalPathValidator(),
                new ChampollionExecutableClassifier(),
                () => roots,
                _ =>
                {
                    if (Interlocked.Increment(ref enteredWorkerCount) >= 2)
                    {
                        workersEntered.Set();
                    }

                    Assert.True(workersEntered.Wait(TimeSpan.FromSeconds(5)), "A second search worker did not start.");
                    return [];
                },
                workerCount: 4);

            string? result = await service.FindAsync(ChampollionEdition.Current);

            Assert.Null(result);
            Assert.True(enteredWorkerCount >= 2);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Theory]
    [InlineData(ChampollionEdition.Legacy, "Legacy")]
    [InlineData(ChampollionEdition.Current, "Current")]
    public async Task Search_returns_only_the_requested_edition(ChampollionEdition edition, string expectedFolder)
    {
        string root = Path.Combine(Path.GetTempPath(), $"ChampollionEditionSearch-{Guid.NewGuid():N}");
        string legacy = Path.Combine(root, "Legacy");
        string current = Path.Combine(root, "Current");
        Directory.CreateDirectory(Path.Combine(legacy, "doc"));
        Directory.CreateDirectory(current);
        foreach (string file in new[] { "Champollion.exe", "Decompiler.dll", "Pex.dll", "vcredist_x64.exe" })
        {
            File.WriteAllText(Path.Combine(legacy, file), string.Empty);
        }
        File.WriteAllText(Path.Combine(legacy, "doc", "Readme.html"), "Champollion V1.0.1 Readme");
        File.WriteAllText(Path.Combine(current, "Champollion.exe"), string.Empty);

        try
        {
            ExecutableSearchService service = new(
                new LocalPathValidator(),
                new ChampollionExecutableClassifier(),
                () => new[] { legacy, current },
                _ => [],
                workerCount: 2);

            string? result = await service.FindAsync(edition);

            Assert.NotNull(result);
            Assert.Equal(expectedFolder, Directory.GetParent(result)!.Name);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Theory]
    [InlineData("Program Files")]
    [InlineData("Program Files (x86)")]
    public async Task Search_traverses_program_files_directories(string programFilesDirectoryName)
    {
        string root = Path.Combine(Path.GetTempPath(), $"ChampollionProgramFilesSearch-{Guid.NewGuid():N}");
        string installDirectory = Path.Combine(root, programFilesDirectoryName, "Champollion");
        Directory.CreateDirectory(installDirectory);
        string executablePath = Path.Combine(installDirectory, "Champollion.exe");
        File.WriteAllText(executablePath, string.Empty);

        try
        {
            ExecutableSearchService service = new(
                new LocalPathValidator(),
                new ChampollionExecutableClassifier(),
                () => [root],
                Directory.EnumerateDirectories,
                workerCount: 2);

            string? result = await service.FindAsync(ChampollionEdition.Current);

            Assert.Equal(executablePath, result);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task Search_traverses_steam_directories()
    {
        string root = Path.Combine(Path.GetTempPath(), $"ChampollionSteamSearch-{Guid.NewGuid():N}");
        string installDirectory = Path.Combine(root, "Steam", "steamapps", "common", "Skyrim", "NewExe");
        Directory.CreateDirectory(installDirectory);
        string executablePath = Path.Combine(installDirectory, "Champollion.exe");
        File.WriteAllText(executablePath, string.Empty);

        try
        {
            ExecutableSearchService service = new(
                new LocalPathValidator(),
                new ChampollionExecutableClassifier(),
                () => [root],
                Directory.EnumerateDirectories,
                workerCount: 2);

            string? result = await service.FindAsync(ChampollionEdition.Current);

            Assert.Equal(executablePath, result);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Theory]
    [InlineData("Windows")]
    [InlineData("ProgramData")]
    public async Task Search_skips_protected_system_directories(string excludedDirectoryName)
    {
        string root = Path.Combine(Path.GetTempPath(), $"ChampollionExcludedSearch-{Guid.NewGuid():N}");
        string excludedDirectory = Path.Combine(root, excludedDirectoryName, "Champollion");
        Directory.CreateDirectory(excludedDirectory);
        File.WriteAllText(Path.Combine(excludedDirectory, "Champollion.exe"), string.Empty);

        try
        {
            ExecutableSearchService service = new(
                new LocalPathValidator(),
                new ChampollionExecutableClassifier(),
                () => [root],
                Directory.EnumerateDirectories,
                workerCount: 2);

            Assert.Null(await service.FindAsync(ChampollionEdition.Current));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

}