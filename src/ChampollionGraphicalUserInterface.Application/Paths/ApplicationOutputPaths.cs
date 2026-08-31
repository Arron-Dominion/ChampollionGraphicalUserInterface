namespace ChampollionGraphicalUserInterface.Application.Paths;

/// <summary>
/// Provides the default output directories owned by the application.
/// </summary>
public static class ApplicationOutputPaths
{
    #region Constants

    /// <summary>
    /// The default Papyrus source output directory name.
    /// </summary>
    public const string SourceDirectoryName = "ChampollionGraphicalUserInterfaceOutput";

    /// <summary>
    /// The default assembly output directory name.
    /// </summary>
    public const string AssemblyDirectoryName = "ChampollionGraphicalUserInterfaceAssembly";

    #endregion

    #region Methods

    /// <summary>
    /// Gets the default Papyrus source output directory.
    /// </summary>
    /// <param name="applicationDirectory">The directory containing the application.</param>
    /// <returns>The absolute default Papyrus source output directory.</returns>
    public static string GetSourceDirectory(string? applicationDirectory = null) =>
        GetDirectory(applicationDirectory, SourceDirectoryName);

    /// <summary>
    /// Gets the default assembly output directory.
    /// </summary>
    /// <param name="applicationDirectory">The directory containing the application.</param>
    /// <returns>The absolute default assembly output directory.</returns>
    public static string GetAssemblyDirectory(string? applicationDirectory = null) =>
        GetDirectory(applicationDirectory, AssemblyDirectoryName);

    /// <summary>
    /// Gets all default output directories.
    /// </summary>
    /// <param name="applicationDirectory">The directory containing the application.</param>
    /// <returns>The absolute default output directories.</returns>
    public static IReadOnlyList<string> GetDirectories(string? applicationDirectory = null) =>
        [GetSourceDirectory(applicationDirectory), GetAssemblyDirectory(applicationDirectory)];

    /// <summary>
    /// Combines an application directory with an owned output directory name.
    /// </summary>
    /// <param name="applicationDirectory">The directory containing the application, or <see langword="null"/> to use the current application base directory.</param>
    /// <param name="directoryName">The owned output directory name.</param>
    /// <returns>The absolute application-owned output directory.</returns>
    private static string GetDirectory(string? applicationDirectory, string directoryName) =>
        Path.GetFullPath(Path.Combine(applicationDirectory ?? AppContext.BaseDirectory, directoryName));

    #endregion
}