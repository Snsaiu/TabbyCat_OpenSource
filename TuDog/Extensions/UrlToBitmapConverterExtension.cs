using System.Globalization;
using Avalonia;
using Avalonia.Media.Imaging;

namespace TuDog.Extensions;

public sealed class UrlToBitmapConverterExtension : ValueConvertBase
{
    private static readonly HttpClient _httpClient = new();

    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string url || string.IsNullOrEmpty(url)) return AvaloniaProperty.UnsetValue;
        try
        {
            // 注意：Bitmap 必须是同步加载，所以这里要偷懒，用 .Result
            var stream = _httpClient.GetStreamAsync(url).Result;
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new Bitmap(memoryStream);
        }
        catch
        {
            return AvaloniaProperty.UnsetValue;
        }
    }
}