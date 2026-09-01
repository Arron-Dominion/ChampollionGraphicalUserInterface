using ChampollionGraphicalUserInterface.Application.DTO.Input;

namespace ChampollionGraphicalUserInterface.Application.Tests.DTO.Input;

public sealed class SavedOptionsTests
{
    [Fact]
    public void Preserves_option_selections()
    {
        SavedOptions options = new()
        {
            GenerateAssembly = true,
            GenerateComments = true,
            Recursive = true,
            RecreateSubdirectories = true,
            WriteHeader = true,
            Trace = true,
            NoDumpTree = true,
            DebugFunctions = true,
            NoDebugLineNumbers = true,
            Verbose = true,
        };

        Assert.True(options.GenerateAssembly);
        Assert.True(options.GenerateComments);
        Assert.True(options.Recursive);
        Assert.True(options.RecreateSubdirectories);
        Assert.True(options.WriteHeader);
        Assert.True(options.Trace);
        Assert.True(options.NoDumpTree);
        Assert.True(options.DebugFunctions);
        Assert.True(options.NoDebugLineNumbers);
        Assert.True(options.Verbose);
    }
}