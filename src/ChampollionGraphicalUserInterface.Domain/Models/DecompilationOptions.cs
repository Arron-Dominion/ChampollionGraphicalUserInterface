namespace ChampollionGraphicalUserInterface.Domain;

/// <summary>
/// Contains optional settings applied to a Champollion decompilation request.
/// </summary>
public sealed record DecompilationOptions
{
    #region Properties

    /// <summary>Gets whether assembly output is generated.</summary>
    /// <value><see langword="true"/> to generate assembly output; otherwise, <see langword="false"/>.</value>
    public bool GenerateAssembly { get; init; }

    /// <summary>Gets the optional assembly output directory.</summary>
    /// <value>The assembly output directory, or <see langword="null"/> to use Champollion's default.</value>
    public string? AssemblyOutputPath { get; init; }

    /// <summary>Gets the optional Papyrus source output directory.</summary>
    /// <value>The source output directory, or <see langword="null"/> to use Champollion's default.</value>
    public string? SourceOutputPath { get; init; }

    /// <summary>Gets whether generated source includes comments.</summary>
    /// <value><see langword="true"/> to generate comments; otherwise, <see langword="false"/>.</value>
    public bool GenerateComments { get; init; }

    /// <summary>Gets whether input directories are processed recursively.</summary>
    /// <value><see langword="true"/> to process subdirectories; otherwise, <see langword="false"/>.</value>
    public bool Recursive { get; init; }

    /// <summary>Gets whether the input directory structure is recreated in the output.</summary>
    /// <value><see langword="true"/> to recreate subdirectories; otherwise, <see langword="false"/>.</value>
    public bool RecreateSubdirectories { get; init; }

    /// <summary>Gets whether generated source includes a header.</summary>
    /// <value><see langword="true"/> to write a header; otherwise, <see langword="false"/>.</value>
    public bool WriteHeader { get; init; }

    /// <summary>Gets whether Champollion tracing is enabled.</summary>
    /// <value><see langword="true"/> to enable tracing; otherwise, <see langword="false"/>.</value>
    public bool Trace { get; init; }

    /// <summary>Gets whether syntax trees are omitted from trace output.</summary>
    /// <value><see langword="true"/> to omit tree dumps; otherwise, <see langword="false"/>.</value>
    public bool NoDumpTree { get; init; }

    /// <summary>Gets whether debug functions are emitted.</summary>
    /// <value><see langword="true"/> to emit debug functions; otherwise, <see langword="false"/>.</value>
    public bool DebugFunctions { get; init; }

    /// <summary>Gets whether debug line numbers are omitted.</summary>
    /// <value><see langword="true"/> to omit debug line numbers; otherwise, <see langword="false"/>.</value>
    public bool NoDebugLineNumbers { get; init; }

    /// <summary>Gets whether verbose output is enabled.</summary>
    /// <value><see langword="true"/> to enable verbose output; otherwise, <see langword="false"/>.</value>
    public bool Verbose { get; init; }

    #endregion
}