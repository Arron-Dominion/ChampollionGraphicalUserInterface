using System.Globalization;
using System.Text.RegularExpressions;
using Avalonia.Data.Converters;

namespace ChampollionGraphicalUserInterface.Converters;

/// <summary>
/// Converts enum member names into space-separated display names.
/// </summary>
public sealed partial class EnumDisplayNameConverter : IValueConverter
{
    #region Methods

    /// <summary>
    /// Converts an enum value into a space-separated display name.
    /// </summary>
    /// <param name="value">The enum value to convert.</param>
    /// <param name="targetType">The type expected by the binding target.</param>
    /// <param name="parameter">An optional converter parameter.</param>
    /// <param name="culture">The culture used by the binding.</param>
    /// <returns>The formatted display name.</returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string name = value?.ToString() ?? string.Empty;
        return WordBoundary().Replace(name, " ");
    }

    /// <summary>
    /// Rejects conversion from a display name back to an enum value.
    /// </summary>
    /// <param name="value">The value supplied by the binding target.</param>
    /// <param name="targetType">The type expected by the binding source.</param>
    /// <param name="parameter">An optional converter parameter.</param>
    /// <param name="culture">The culture used by the binding.</param>
    /// <returns>This method does not return because reverse conversion is unsupported.</returns>
    /// <exception cref="NotSupportedException">Always thrown because reverse conversion is unsupported.</exception>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();

    /// <summary>
    /// Gets a regular expression that identifies display-name word boundaries.
    /// </summary>
    /// <returns>The generated word-boundary regular expression.</returns>
    [GeneratedRegex("(?<=[a-z])(?=[A-Z0-9])|(?<=\\d)(?=[A-Z])")]
    private static partial Regex WordBoundary();

    #endregion
}
