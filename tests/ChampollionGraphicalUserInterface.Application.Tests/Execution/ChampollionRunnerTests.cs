using System.Text;
using ChampollionGraphicalUserInterface.Application.DTO.Output;
using ChampollionGraphicalUserInterface.Application.Execution;

namespace ChampollionGraphicalUserInterface.Application.Tests.Execution;

public sealed class ChampollionRunnerTests
{
    [Theory]
    [InlineData(0, true)]
    [InlineData(1, false)]
    public void File_execution_result_success_reflects_exit_code(int exitCode, bool expected)
    {
        FileExecutionResult result = ChampollionRunner.CreateFileExecutionResult(
            "script.pex", exitCode, "output", "error");

        Assert.Equal(expected, result.Succeeded);
    }

    [Fact]
    public void Execution_summary_counts_successful_and_failed_results()
    {
        FileExecutionResult[] results =
        [
            ChampollionRunner.CreateFileExecutionResult("success.pex", 0, "", ""),
            ChampollionRunner.CreateFileExecutionResult("failure.pex", 1, "", "error"),
        ];

        ExecutionSummary summary = ChampollionRunner.CreateExecutionSummary(results, "diagnostic.log");

        Assert.Equal(1, summary.SuccessfulCount);
        Assert.Equal(1, summary.FailedCount);
        Assert.Equal("diagnostic.log", summary.LogPath);
    }

    [Fact]
    public async Task Stream_reader_reports_output_and_retains_complete_text()
    {
        const string processOutput = "Starting decompilation...\r\nFinished script.pex\r\n";
        await using MemoryStream stream = new(Encoding.UTF8.GetBytes(processOutput));
        using StreamReader reader = new(stream);
        List<ExecutionOutput> updates = [];
        InlineProgress<ExecutionOutput> progress = new(updates.Add);

        string captured = await ChampollionRunner.ReadStreamAsync(
            reader, "script.pex", false, progress, CancellationToken.None);

        Assert.Equal(processOutput, captured);
        Assert.NotEmpty(updates);
        Assert.Equal(processOutput, string.Concat(updates.Select(update => update.Text)));
        Assert.All(updates, update => Assert.False(update.IsError));
    }

    private sealed class InlineProgress<T>(Action<T> report) : IProgress<T>
    {
        public void Report(T value) => report(value);
    }
}