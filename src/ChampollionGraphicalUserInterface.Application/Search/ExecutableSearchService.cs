using System.Collections.Concurrent;
using ChampollionGraphicalUserInterface.Application.DTO.Output;
using ChampollionGraphicalUserInterface.Application.Validation;
using ChampollionGraphicalUserInterface.Domain;

namespace ChampollionGraphicalUserInterface.Application.Search;

/// <summary>
/// Searches local directories concurrently for a compatible Champollion executable.
/// </summary>
public sealed class ExecutableSearchService
{
    #region Variables

    /// <summary>
    /// The file name expected for a Champollion executable.
    /// </summary>
    public const string ExpectedExecutableFileName = "Champollion.exe";

    /// <summary>
    /// Directory names excluded from recursive executable searches.
    /// </summary>
    private static readonly string[] ExcludedDirectoryNames =
        ["Windows", "ProgramData", "Microsoft", "Visual Studio"];

    /// <summary>
    /// Maximum number of concurrent directory search workers.
    /// </summary>
    private const int MaximumWorkerCount = 32;

    /// <summary>
    /// The validator used to confirm executable candidates are valid local executable paths.
    /// </summary>
    private readonly LocalPathValidator pathValidator;

    /// <summary>
    /// Provides the root directories from which each search begins.
    /// </summary>
    private readonly Func<IEnumerable<string>> startingDirectories;

    /// <summary>
    /// Enumerates child directories for a searched directory.
    /// </summary>
    private readonly Func<string, IEnumerable<string>> enumerateDirectories;

    /// <summary>
    /// The number of concurrent directory search workers.
    /// </summary>
    private readonly int workerCount;

    /// <summary>
    /// The classifier used to match executable candidates to an edition.
    /// </summary>
    private readonly ChampollionExecutableClassifier classifier;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ExecutableSearchService"/> class using system search defaults.
    /// </summary>
    /// <param name="pathValidator">The validator used to validate executable candidates.</param>
    public ExecutableSearchService(LocalPathValidator pathValidator)
        : this(pathValidator, new ChampollionExecutableClassifier(), GetStartingDirectories, Directory.EnumerateDirectories,
            CalculateWorkerCount(Environment.ProcessorCount))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExecutableSearchService"/> class using supplied search dependencies.
    /// </summary>
    /// <param name="pathValidator">The validator used to validate executable candidates.</param>
    /// <param name="classifier">The classifier used to match executable candidates to an edition.</param>
    /// <param name="startingDirectories">A function that provides search root directories.</param>
    /// <param name="enumerateDirectories">A function that enumerates child directories.</param>
    /// <param name="workerCount">The number of concurrent search workers.</param>
    internal ExecutableSearchService(
        LocalPathValidator pathValidator,
        ChampollionExecutableClassifier classifier,
        Func<IEnumerable<string>> startingDirectories,
        Func<string, IEnumerable<string>> enumerateDirectories,
        int workerCount)
    {
        this.pathValidator = pathValidator;
        this.classifier = classifier;
        this.startingDirectories = startingDirectories;
        this.enumerateDirectories = enumerateDirectories;
        this.workerCount = workerCount;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Calculates a bounded worker count for the blocking filesystem search workload.
    /// </summary>
    /// <param name="processorCount">The number of logical processors available to the process.</param>
    /// <returns>The number of directory search workers to create.</returns>
    internal static int CalculateWorkerCount(int processorCount) =>
        Math.Clamp(processorCount * 2, 4, MaximumWorkerCount);

    /// <summary>
    /// Searches local directories for a Champollion executable matching an edition.
    /// </summary>
    /// <param name="edition">The Champollion edition to find.</param>
    /// <param name="progress">An optional receiver for directory search progress.</param>
    /// <param name="cancellationToken">The token used to cancel the search.</param>
    /// <returns>A task containing the first matching executable path, or <see langword="null"/> when none is found.</returns>
    public async Task<string?> FindAsync(
        ChampollionEdition edition,
        IProgress<SearchProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        using CancellationTokenSource searchCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        ConcurrentQueue<string> work = new();
        ConcurrentDictionary<string, byte> visited = new(StringComparer.OrdinalIgnoreCase);
        using SemaphoreSlim available = new(0);
        TaskCompletionSource<string?> result = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int pending = 0;
        int directoriesSearched = 0;
        int activeWorkers = 0;

        void Enqueue(string directory)
        {
            if (!visited.TryAdd(directory, 0))
            {
                return;
            }

            Interlocked.Increment(ref pending);
            work.Enqueue(directory);
            available.Release();
        }

        foreach (string root in startingDirectories().Where(Directory.Exists))
        {
            Enqueue(root);
        }

        if (Volatile.Read(ref pending) == 0)
        {
            return null;
        }

        Task[] workers = Enumerable.Range(0, workerCount).Select(_ => Task.Factory.StartNew(() =>
        {
            try
            {
                while (true)
                {
                    available.Wait(searchCancellation.Token);
                    if (!work.TryDequeue(out string? directory))
                    {
                        continue;
                    }

                    int currentActiveWorkers = Interlocked.Increment(ref activeWorkers);
                    int currentDirectoriesSearched = Interlocked.Increment(ref directoriesSearched);
                    progress?.Report(new SearchProgress(currentDirectoriesSearched, currentActiveWorkers, workerCount));
                    try
                    {
                        string candidate = Path.Combine(directory, ExpectedExecutableFileName);
                        if (pathValidator.ValidateExecutable(candidate).IsValid && classifier.Matches(candidate, edition))
                        {
                            result.TrySetResult(candidate);
                            searchCancellation.Cancel();
                            return;
                        }

                        try
                        {
                            foreach (string child in enumerateDirectories(directory))
                            {
                                if (!IsExcluded(child))
                                {
                                    Enqueue(child);
                                }
                            }
                        }
                        catch (Exception exception) when (exception is UnauthorizedAccessException or IOException)
                        {
                        }
                    }
                    finally
                    {
                        Interlocked.Decrement(ref activeWorkers);
                        if (Interlocked.Decrement(ref pending) == 0)
                        {
                            result.TrySetResult(null);
                            searchCancellation.Cancel();
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default)).ToArray();

        try
        {
            return await result.Task.WaitAsync(cancellationToken);
        }
        finally
        {
            searchCancellation.Cancel();
            await Task.WhenAll(workers);
        }
    }

    /// <summary>
    /// Enumerates the default application, user, and fixed-drive search roots.
    /// </summary>
    /// <returns>The existing or potentially available root directories to search.</returns>
    private static IEnumerable<string> GetStartingDirectories()
    {
        string applicationDirectory = AppContext.BaseDirectory;
        yield return applicationDirectory;

        foreach (Environment.SpecialFolder folder in new[]
        {
            Environment.SpecialFolder.UserProfile,
            Environment.SpecialFolder.DesktopDirectory,
            Environment.SpecialFolder.MyDocuments,
        })
        {
            string path = Environment.GetFolderPath(folder);
            if (Directory.Exists(path))
            {
                yield return path;
            }
        }

        string downloads = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        if (Directory.Exists(downloads))
        {
            yield return downloads;
        }

        foreach (DriveInfo drive in DriveInfo.GetDrives().Where(drive => drive.IsReady && drive.DriveType == DriveType.Fixed))
        {
            yield return drive.RootDirectory.FullName;
        }
    }

    /// <summary>
    /// Determines whether a directory path should be excluded from the search.
    /// </summary>
    /// <param name="path">The directory path to evaluate.</param>
    /// <returns><see langword="true"/> when the directory name is excluded; otherwise, <see langword="false"/>.</returns>
    private static bool IsExcluded(string path)
    {
        string name = Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar));
        return ExcludedDirectoryNames.Contains(name, StringComparer.OrdinalIgnoreCase);
    }

    #endregion
}