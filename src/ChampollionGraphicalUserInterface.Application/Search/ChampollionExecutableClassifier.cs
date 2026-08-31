using System.Diagnostics;
using ChampollionGraphicalUserInterface.Application.Enums;
using ChampollionGraphicalUserInterface.Domain;

namespace ChampollionGraphicalUserInterface.Application.Search;

/// <summary>
/// Classifies Champollion executables by inspecting installation markers and version information.
/// </summary>
public sealed class ChampollionExecutableClassifier
{
    #region Variables

    /// <summary>
    /// Files expected alongside a complete legacy Champollion installation.
    /// </summary>
    private static readonly string[] LegacyCompanionFiles = ["Decompiler.dll", "Pex.dll", "vcredist_x64.exe"];

    #endregion

    #region Methods

    /// <summary>
    /// Determines the Champollion edition represented by an executable path.
    /// </summary>
    /// <param name="executablePath">The executable path to classify.</param>
    /// <returns>The detected executable classification.</returns>
    public ExecutableClassification Classify(string executablePath)
    {
        if (!File.Exists(executablePath))
        {
            return ExecutableClassification.Unknown;
        }

        string directory = Path.GetDirectoryName(executablePath)!;
        bool hasAllLegacyCompanions = LegacyCompanionFiles.All(file => File.Exists(Path.Combine(directory, file)));
        bool hasLegacyReadme = File.Exists(Path.Combine(directory, "doc", "Readme.html"));
        bool hasAnyLegacyMarker = hasLegacyReadme || LegacyCompanionFiles.Any(file => File.Exists(Path.Combine(directory, file)));

        if (hasAllLegacyCompanions && hasLegacyReadme)
        {
            return ExecutableClassification.Legacy;
        }

        if (hasAnyLegacyMarker)
        {
            return ExecutableClassification.Unknown;
        }

        FileVersionInfo version = FileVersionInfo.GetVersionInfo(executablePath);
        if (version.FileMajorPart == 1 && version.FileMinorPart == 0)
        {
            return ExecutableClassification.Unknown;
        }

        return ExecutableClassification.Current;
    }

    /// <summary>
    /// Determines whether an executable matches an expected Champollion edition.
    /// </summary>
    /// <param name="executablePath">The executable path to classify.</param>
    /// <param name="edition">The expected Champollion edition.</param>
    /// <returns><see langword="true"/> when the executable is reliably classified as the expected edition; otherwise, <see langword="false"/>.</returns>
    public bool Matches(string executablePath, ChampollionEdition edition) => Classify(executablePath) switch
    {
        ExecutableClassification.Legacy => edition == ChampollionEdition.Legacy,
        ExecutableClassification.Current => edition == ChampollionEdition.Current,
        _ => false,
    };

    #endregion
}