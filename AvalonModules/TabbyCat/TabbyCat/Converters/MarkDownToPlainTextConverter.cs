using System.Globalization;
using TuDog.Extensions;

namespace TabbyCat.Converters;

public sealed class MarkDownToPlainTextConverter : ValueConvertBase
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Markdig.Markdown.ToPlainText(value?.ToString() ?? string.Empty);
    }
}