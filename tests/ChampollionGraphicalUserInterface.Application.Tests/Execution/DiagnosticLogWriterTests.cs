using ChampollionGraphicalUserInterface.Application.DTO.Output;
using ChampollionGraphicalUserInterface.Application.Execution;
using ChampollionGraphicalUserInterface.Domain;

namespace ChampollionGraphicalUserInterface.Application.Tests.Execution;

public sealed class DiagnosticLogWriterTests
{
    [Fact]
    public async Task Write_creates_a_log_with_request_and_result_details()
    {
        string applicationDirectory = Path.Combine(Path.GetTempPath(), $"ChampollionLog-{Guid.NewGuid():N}");
        ChampollionRequest request = new(
            ChampollionEdition.Current,
            SupportedGame.Starfield,
            ChampollionOperation.Decompile,
            "Champollion.exe",
            "script.pex",
            new DecompilationOptions());
        FileExecutionResult result = new("script.pex", 1, "standard output", "standard error", false);

        try
        {
            DiagnosticLogWriter writer = new(applicationDirectory);

            string logPath = await writer.WriteAsync(request, [result]);
            string contents = await File.ReadAllTextAsync(logPath);

            Assert.True(File.Exists(logPath));
            Assert.Contains("Edition: Current", contents);
            Assert.Contains("Game: Starfield", contents);
            Assert.Contains("Input: script.pex", contents);
            Assert.Contains("Exit code: 1", contents);
            Assert.Contains("standard output", contents);
            Assert.Contains("standard error", contents);
        }
        finally
        {
            if (Directory.Exists(applicationDirectory))
            {
                Directory.Delete(applicationDirectory, recursive: true);
            }
        }
    }
}