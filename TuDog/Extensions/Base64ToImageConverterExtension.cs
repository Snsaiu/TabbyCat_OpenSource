using System.Globalization;
using Avalonia.Media.Imaging;

namespace TuDog.Extensions;

public sealed class Base64ToImageConverterExtension : ValueConvertBase
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string image) return null;

        var imageBytes = System.Convert.FromBase64String(image);
        using var ms = new MemoryStream(imageBytes);
        return new Bitmap(ms);
    }
}