using ChampollionGraphicalUserInterface.Application.DTO.Output;

namespace ChampollionGraphicalUserInterface.Application.Validation;

/// <summary>
/// Validates local input, executable, and output paths used by Champollion operations.
/// </summary>
public sealed class LocalPathValidator
{
    #region Variables

    /// <summary>
    /// Environment variables whose resolved directories are protected from output creation.
    /// </summary>
    private static readonly string[] ProtectedEnvironmentPaths =
        ["WINDIR", "ProgramFiles", "ProgramFiles(x86)", "ProgramData"];

    /// <summary>Protected output roots explicitly owned and made writable by the application.</summary>
    private readonly string[] allowedProtectedOutputRoots;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalPathValidator"/> class.
    /// </summary>
    /// <param name="allowedProtectedOutputRoots">Application-owned roots that may receive output in a protected location.</param>
    public LocalPathValidator(IEnumerable<string>? allowedProtectedOutputRoots = null)
    {
        this.allowedProtectedOutputRoots = allowedProtectedOutputRoots?
            .Select(Path.GetFullPath)
            .ToArray() ?? [];
    }

    #endregion

    #region Methods

    /// <summary>
    /// Validates that an input path identifies an existing local directory or PEX file.
    /// </summary>
    /// <param name="path">The input path to validate.</param>
    /// <returns>The path validation result.</returns>
    public PathValidationResult ValidateInput(string? path)
    {
        PathValidationResult localPath = ValidateLocalPath(path);
        if (!localPath.IsValid)
        {
            return localPath;
        }

        string expandedPath = localPath.ExpandedPath!;
        if (!File.Exists(expandedPath) && !Directory.Exists(expandedPath))
        {
            return Invalid("The input file or directory does not exist.");
        }

        if (File.Exists(expandedPath) && !string.Equals(Path.GetExtension(expandedPath), ".pex", StringComparison.OrdinalIgnoreCase))
        {
            return Invalid("Input files must use the .pex extension.");
        }

        return localPath;
    }

    /// <summary>
    /// Validates that a path identifies an existing local Windows executable.
    /// </summary>
    /// <param name="path">The executable path to validate.</param>
    /// <returns>The path validation result.</returns>
    public PathValidationResult ValidateExecutable(string? path)
    {
        PathValidationResult localPath = ValidateLocalPath(path);
        if (!localPath.IsValid)
        {
            return localPath;
        }

        string expandedPath = localPath.ExpandedPath!;
        if (!File.Exists(expandedPath) || !string.Equals(Path.GetExtension(expandedPath), ".exe", StringComparison.OrdinalIgnoreCase))
        {
            return Invalid("Select an existing Windows executable (.exe).");
        }

        return localPath;
    }

    /// <summary>
    /// Validates that an output path is local, has an existing parent, and is outside protected locations.
    /// </summary>
    /// <param name="path">The output path to validate.</param>
    /// <returns>The path validation result.</returns>
    public PathValidationResult ValidateOutput(string? path)
    {
        PathValidationResult localPath = ValidateLocalPath(path);
        if (!localPath.IsValid)
        {
            return localPath;
        }

        string expandedPath = localPath.ExpandedPath!;
        if (IsProtected(expandedPath) && !IsAllowedProtectedOutput(expandedPath))
        {
            return Invalid("Output cannot be created in a protected Windows location.");
        }

        string? existingParent = expandedPath;
        while (!string.IsNullOrWhiteSpace(existingParent) && !Directory.Exists(existingParent))
        {
            existingParent = Path.GetDirectoryName(existingParent);
        }

        return existingParent is null
            ? Invalid("The output path has no existing parent directory.")
            : localPath;
    }

    /// <summary>
    /// Expands and validates the common requirements for a local fixed-drive path.
    /// </summary>
    /// <param name="path">The path to expand and validate.</param>
    /// <returns>The path validation result.</returns>
    private static PathValidationResult ValidateLocalPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return Invalid("A path is required.");
        }

        string expandedPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables(path.Trim().Trim('"')));
        if (!Path.IsPathFullyQualified(expandedPath))
        {
            return Invalid("Use an absolute path.");
        }

        if (expandedPath.StartsWith("\\\\", StringComparison.Ordinal))
        {
            return Invalid("Network paths are not supported.");
        }

        string? root = Path.GetPathRoot(expandedPath);
        if (string.IsNullOrWhiteSpace(root))
        {
            return Invalid("The path has no local drive.");
        }

        DriveInfo drive = new(root);
        if (drive.DriveType != DriveType.Fixed)
        {
            return Invalid("Only local fixed drives are supported.");
        }

        return Valid(expandedPath);
    }

    /// <summary>
    /// Creates a successful path validation result.
    /// </summary>
    /// <param name="path">The expanded valid path.</param>
    /// <returns>A successful path validation result.</returns>
    private static PathValidationResult Valid(string path) => new(true, path, null);

    /// <summary>
    /// Creates a failed path validation result.
    /// </summary>
    /// <param name="error">The validation error message.</param>
    /// <returns>A failed path validation result.</returns>
    private static PathValidationResult Invalid(string error) => new(false, null, error);

    /// <summary>
    /// Determines whether a path starts within a protected Windows location.
    /// </summary>
    /// <param name="path">The expanded path to evaluate.</param>
    /// <returns><see langword="true"/> when the path is protected; otherwise, <see langword="false"/>.</returns>
    private static bool IsProtected(string path)
    {
        foreach (string variable in ProtectedEnvironmentPaths)
        {
            string? protectedPath = Environment.GetEnvironmentVariable(variable);
            if (!string.IsNullOrWhiteSpace(protectedPath) &&
                path.StartsWith(Path.GetFullPath(protectedPath), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether an output path is within an application-owned protected root.
    /// </summary>
    /// <param name="path">The expanded output path to evaluate.</param>
    /// <returns><see langword="true"/> when the path is explicitly allowed; otherwise, <see langword="false"/>.</returns>
    private bool IsAllowedProtectedOutput(string path)
    {
        string pathWithSeparator = Path.TrimEndingDirectorySeparator(path) + Path.DirectorySeparatorChar;
        return allowedProtectedOutputRoots.Any(root =>
        {
            string normalizedRoot = Path.TrimEndingDirectorySeparator(root);
            return string.Equals(path, normalizedRoot, StringComparison.OrdinalIgnoreCase) ||
                pathWithSeparator.StartsWith(normalizedRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
        });
    }

    #endregion
}