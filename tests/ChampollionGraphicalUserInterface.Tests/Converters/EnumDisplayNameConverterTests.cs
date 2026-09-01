using System.Globalization;
using ChampollionGraphicalUserInterface.Converters;
using ChampollionGraphicalUserInterface.Domain;

namespace ChampollionGraphicalUserInterface.Tests.Converters;

public sealed class EnumDisplayNameConverterTests
{
    [Theory]
    [InlineData(SupportedGame.SkyrimSpecialEdition, "Skyrim Special Edition")]
    [InlineData(SupportedGame.Fallout4, "Fallout 4")]
    [InlineData(SupportedGame.Fallout76, "Fallout 76")]
    public void Converts_enum_names_to_human_readable_text(SupportedGame game, string expected)
    {
        EnumDisplayNameConverter converter = new();

        Assert.Equal(expected, converter.Convert(game, typeof(string), null, CultureInfo.InvariantCulture));
    }
}