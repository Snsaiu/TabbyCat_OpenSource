using System.Globalization;
using Avalonia.Data;
using TuDog.Extensions;

namespace TabbyCat.Converters;

public sealed class String2FirstCharConverter:ValueConvertBase
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not null )
            return value.ToString().FirstOrDefault();
        return string.Empty;
    }
}