namespace ChampollionGraphicalUserInterface.Domain;

/// <summary>
/// Identifies an operation exposed by the Champollion command-line tool.
/// </summary>
public enum ChampollionOperation
{
    /// <summary>Decompiles one or more PEX files.</summary>
    Decompile,

    /// <summary>Displays command-line help.</summary>
    Help,

    /// <summary>Displays the Champollion version.</summary>
    Version,

    /// <summary>Prints information about a PEX file.</summary>
    PrintInformation,

    /// <summary>Prints compile-time information about a PEX file.</summary>
    PrintCompileTime,
}