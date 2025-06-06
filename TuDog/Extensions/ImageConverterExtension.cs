using System.Globalization;
using Avalonia.Media.Imaging;

namespace TuDog.Extensions;

public sealed class ImageConverterExtension : ValueConvertBase
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string image) return null;

        return File.Exists(image) ? new Bitmap(image) : null;
    }
}