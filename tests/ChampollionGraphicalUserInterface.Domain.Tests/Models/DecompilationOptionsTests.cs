using ChampollionGraphicalUserInterface.Domain;

namespace ChampollionGraphicalUserInterface.Domain.Tests.Models;

public sealed class DecompilationOptionsTests
{
    [Fact]
    public void Preserves_initialized_values()
    {
        DecompilationOptions options = new()
        {
            GenerateAssembly = true,
            AssemblyOutputPath = @"C:\Output\Assembly",
            SourceOutputPath = @"C:\Output\Source",
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
        Assert.Equal(@"C:\Output\Assembly", options.AssemblyOutputPath);
        Assert.Equal(@"C:\Output\Source", options.SourceOutputPath);
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

    [Fact]
    public void Defaults_to_unselected_and_no_paths()
    {
        DecompilationOptions options = new();

        Assert.False(options.GenerateAssembly);
        Assert.Null(options.AssemblyOutputPath);
        Assert.Null(options.SourceOutputPath);
        Assert.False(options.GenerateComments);
        Assert.False(options.Recursive);
        Assert.False(options.RecreateSubdirectories);
        Assert.False(options.WriteHeader);
        Assert.False(options.Trace);
        Assert.False(options.NoDumpTree);
        Assert.False(options.DebugFunctions);
        Assert.False(options.NoDebugLineNumbers);
        Assert.False(options.Verbose);
    }

    [Fact]
    public void Supports_non_destructive_updates()
    {
        DecompilationOptions original = new() { GenerateAssembly = true };

        DecompilationOptions updated = original with { Verbose = true };

        Assert.False(original.Verbose);
        Assert.True(updated.GenerateAssembly);
        Assert.True(updated.Verbose);
    }
}