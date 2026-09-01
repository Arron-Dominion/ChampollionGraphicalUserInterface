using ChampollionGraphicalUserInterface.Domain;

namespace ChampollionGraphicalUserInterface.Domain.Tests.Models;

public sealed class ChampollionRequestTests
{
    [Fact]
    public void Constructor_preserves_values()
    {
        DecompilationOptions options = new() { GenerateComments = true };

        ChampollionRequest request = new(
            ChampollionEdition.Current,
            SupportedGame.Starfield,
            ChampollionOperation.Decompile,
            @"C:\Tools\Champollion.exe",
            @"C:\Scripts\Example.pex",
            options);

        Assert.Equal(ChampollionEdition.Current, request.Edition);
        Assert.Equal(SupportedGame.Starfield, request.Game);
        Assert.Equal(ChampollionOperation.Decompile, request.Operation);
        Assert.Equal(@"C:\Tools\Champollion.exe", request.ExecutablePath);
        Assert.Equal(@"C:\Scripts\Example.pex", request.InputPath);
        Assert.Same(options, request.Options);
    }

    [Fact]
    public void Supports_non_destructive_updates()
    {
        ChampollionRequest original = new(
            ChampollionEdition.Legacy,
            SupportedGame.Skyrim,
            ChampollionOperation.Help,
            "Champollion.exe",
            null,
            new DecompilationOptions());

        ChampollionRequest updated = original with { Edition = ChampollionEdition.Current };

        Assert.Equal(ChampollionEdition.Legacy, original.Edition);
        Assert.Equal(ChampollionEdition.Current, updated.Edition);
    }
}