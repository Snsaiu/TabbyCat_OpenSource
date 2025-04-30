using System.Globalization;
using System.IO;
using Avalonia;
using LibVLCSharp.Shared;
using TuDog.Extensions;

namespace TabbyCat.Converters;

public sealed class StringToVideoMediaConverter : ValueConvertBase
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str && parameter is MediaPlayer mp)
        {
            if (!File.Exists(str))
                return AvaloniaProperty.UnsetValue;
            return new Media(null, str);
        }

        return AvaloniaProperty.UnsetValue;
    }
}