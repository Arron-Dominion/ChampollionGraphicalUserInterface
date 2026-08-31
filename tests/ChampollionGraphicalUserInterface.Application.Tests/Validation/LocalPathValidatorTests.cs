using ChampollionGraphicalUserInterface.Application.DTO.Output;
using ChampollionGraphicalUserInterface.Application.Validation;

namespace ChampollionGraphicalUserInterface.Application.Tests.Validation;

public sealed class LocalPathValidatorTests
{
    private readonly LocalPathValidator validator = new();

    [Fact]
    public void Input_expands_environment_variables()
    {
        string temporaryFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.pex");
        File.WriteAllText(temporaryFile, string.Empty);
        Environment.SetEnvironmentVariable("CHAMPOLLION_TEST_INPUT", temporaryFile);
        try
        {
            PathValidationResult result = validator.ValidateInput("%CHAMPOLLION_TEST_INPUT%");

            Assert.True(result.IsValid, result.Error);
            Assert.Equal(temporaryFile, result.ExpandedPath);
        }
        finally
        {
            File.Delete(temporaryFile);
            Environment.SetEnvironmentVariable("CHAMPOLLION_TEST_INPUT", null);
        }
    }

    [Fact]
    public void Unc_paths_are_rejected()
    {
        PathValidationResult result = validator.ValidateInput(@"\\server\share\script.pex");

        Assert.False(result.IsValid);
        Assert.Contains("Network", result.Error);
    }

    [Fact]
    public void Application_owned_output_is_allowed_in_protected_location()
    {
        string protectedDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        string allowedOutput = Path.Combine(protectedDirectory, $"ChampollionOutput-{Guid.NewGuid():N}");
        LocalPathValidator protectedOutputValidator = new([allowedOutput]);

        PathValidationResult result = protectedOutputValidator.ValidateOutput(allowedOutput);

        Assert.True(result.IsValid, result.Error);
    }

    [Fact]
    public void Unowned_output_remains_rejected_in_protected_location()
    {
        string protectedDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        string allowedOutput = Path.Combine(protectedDirectory, $"ChampollionOutput-{Guid.NewGuid():N}");
        LocalPathValidator protectedOutputValidator = new([allowedOutput]);

        PathValidationResult result = protectedOutputValidator.ValidateOutput(Path.Combine(protectedDirectory, "OtherOutput"));

        Assert.False(result.IsValid);
        Assert.Contains("protected", result.Error);
    }
}