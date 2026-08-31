using ChampollionGraphicalUserInterface.Application.Enums;
using ChampollionGraphicalUserInterface.Application.Search;

namespace ChampollionGraphicalUserInterface.Application.Tests.Search;

public sealed class ChampollionExecutableClassifierTests
{
    [Fact]
    public void Distinguishes_supplied_legacy_layout_from_standalone_current_layout()
    {
        string root = Path.Combine(Path.GetTempPath(), $"ChampollionClassification-{Guid.NewGuid():N}");
        string legacy = Path.Combine(root, "Legacy");
        string current = Path.Combine(root, "Current");
        Directory.CreateDirectory(Path.Combine(legacy, "doc"));
        Directory.CreateDirectory(current);
        foreach (string file in new[] { "Champollion.exe", "Decompiler.dll", "Pex.dll", "vcredist_x64.exe" })
        {
            File.WriteAllText(Path.Combine(legacy, file), string.Empty);
        }
        File.WriteAllText(Path.Combine(legacy, "doc", "Readme.html"), "Champollion V1.0.1 Readme");
        File.WriteAllText(Path.Combine(current, "Champollion.exe"), string.Empty);

        try
        {
            ChampollionExecutableClassifier classifier = new();

            Assert.Equal(ExecutableClassification.Legacy, classifier.Classify(Path.Combine(legacy, "Champollion.exe")));
            Assert.Equal(ExecutableClassification.Current, classifier.Classify(Path.Combine(current, "Champollion.exe")));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}