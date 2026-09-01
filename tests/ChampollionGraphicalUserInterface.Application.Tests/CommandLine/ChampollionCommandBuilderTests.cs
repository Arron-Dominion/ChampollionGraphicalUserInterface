using ChampollionGraphicalUserInterface.Application.CommandLine;
using ChampollionGraphicalUserInterface.Domain;

namespace ChampollionGraphicalUserInterface.Application.Tests.CommandLine;

public sealed class ChampollionCommandBuilderTests
{
    [Fact]
    public void Paths_with_spaces_remain_single_argument_list_entries()
    {
        ChampollionRequest request = new(ChampollionEdition.Current, SupportedGame.Starfield,
            ChampollionOperation.Decompile, @"C:\Program Files\Champollion\Champollion.exe",
            @"C:\My Mod\script.pex", new DecompilationOptions
            {
                SourceOutputPath = @"C:\My Mod\Source Output",
                GenerateAssembly = true,
                AssemblyOutputPath = @"C:\My Mod\Assembly Output",
            });

        IReadOnlyList<string> arguments = ChampollionCommandBuilder.BuildArguments(request);

        Assert.Equal(@"C:\My Mod\script.pex", arguments[0]);
        Assert.Contains(@"C:\My Mod\Source Output", arguments);
        Assert.Contains(@"C:\My Mod\Assembly Output", arguments);
        Assert.DoesNotContain(arguments, argument => argument.StartsWith('"'));
    }

    [Theory]
    [InlineData(ChampollionOperation.Help, "--help")]
    [InlineData(ChampollionOperation.Version, "--version")]
    public void Standalone_operations_emit_only_their_flag(ChampollionOperation operation, string flag)
    {
        ChampollionRequest request = new(ChampollionEdition.Current, SupportedGame.Starfield,
            operation, "Champollion.exe", null, new DecompilationOptions { Verbose = true });

        Assert.Equal([flag], ChampollionCommandBuilder.BuildArguments(request));
    }

    [Fact]
    public void Print_information_is_distinct_from_decompilation_options()
    {
        ChampollionRequest request = new(ChampollionEdition.Current, SupportedGame.Fallout4,
            ChampollionOperation.PrintInformation, "Champollion.exe", "script.pex",
            new DecompilationOptions { GenerateComments = true, Recursive = true });

        Assert.Equal(["script.pex", "--print-info"], ChampollionCommandBuilder.BuildArguments(request));
    }

    [Fact]
    public void Builder_preserves_each_expanded_output_path_as_one_argument()
    {
        string outputPath = Path.Combine(Path.GetTempPath(), "Champollion Output");
        ChampollionRequest request = new(ChampollionEdition.Current, SupportedGame.Starfield,
            ChampollionOperation.Decompile, "Champollion.exe", "script.pex",
            new DecompilationOptions { SourceOutputPath = outputPath });

        IReadOnlyList<string> arguments = ChampollionCommandBuilder.BuildArguments(request);

        Assert.Equal(outputPath, arguments[2]);
    }
}