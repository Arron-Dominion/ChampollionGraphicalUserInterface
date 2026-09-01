namespace ChampollionGraphicalUserInterface.Application.DTO.Input;

/// <summary>
/// Stores the decompilation options selected for an edition and game profile.
/// </summary>
public sealed record SavedOptions
{
    #region Properties

    /// <summary>
    /// Gets whether assembly output is generated.
    /// </summary>
    /// <value><see langword="true"/> to generate assembly output; otherwise, <see langword="false"/>.</value>
    public bool GenerateAssembly { get; init; }

    /// <summary>
    /// Gets whether comments are generated.
    /// </summary>
    /// <value><see langword="true"/> to generate comments; otherwise, <see langword="false"/>.</value>
    public bool GenerateComments { get; init; }

    /// <summary>
    /// Gets whether input directories are searched recursively.
    /// </summary>
    /// <value><see langword="true"/> to search recursively; otherwise, <see langword="false"/>.</value>
    public bool Recursive { get; init; }

    /// <summary>
    /// Gets whether source subdirectories are recreated in the output.
    /// </summary>
    /// <value><see langword="true"/> to recreate subdirectories; otherwise, <see langword="false"/>.</value>
    public bool RecreateSubdirectories { get; init; }

    /// <summary>
    /// Gets whether a header is written to generated source files.
    /// </summary>
    /// <value><see langword="true"/> to write headers; otherwise, <see langword="false"/>.</value>
    public bool WriteHeader { get; init; }

    /// <summary>
    /// Gets whether decompiler tracing is enabled.
    /// </summary>
    /// <value><see langword="true"/> to enable tracing; otherwise, <see langword="false"/>.</value>
    public bool Trace { get; init; }

    /// <summary>
    /// Gets whether syntax tree output is suppressed while tracing.
    /// </summary>
    /// <value><see langword="true"/> to suppress tree output; otherwise, <see langword="false"/>.</value>
    public bool NoDumpTree { get; init; }

    /// <summary>
    /// Gets whether debug function information is emitted.
    /// </summary>
    /// <value><see langword="true"/> to emit debug function information; otherwise, <see langword="false"/>.</value>
    public bool DebugFunctions { get; init; }

    /// <summary>
    /// Gets whether debug line numbers are omitted.
    /// </summary>
    /// <value><see langword="true"/> to omit debug line numbers; otherwise, <see langword="false"/>.</value>
    public bool NoDebugLineNumbers { get; init; }

    /// <summary>
    /// Gets whether verbose output is enabled.
    /// </summary>
    /// <value><see langword="true"/> to enable verbose output; otherwise, <see langword="false"/>.</value>
    public bool Verbose { get; init; }

    #endregion
}