using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;

namespace TuDog.Extensions;

public class AsyncBitmapBindingExtension : MarkupExtension
{
    public string? Url { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget provideValueTarget
            && provideValueTarget.TargetObject is Image image)
            // 绑定完了之后异步加载图片
            _ = LoadImageAsync(image);
        return null!;
    }

    private async Task LoadImageAsync(Image image)
    {
        if (!string.IsNullOrEmpty(Url))
        {
            var bitmap = await UrlBitmapLoader.LoadBitmapAsync(Url);
            if (bitmap != null) image.Source = bitmap;
        }
    }
}

public static class UrlBitmapLoader
{
    private static readonly HttpClient _httpClient = new();

    public static async Task<Bitmap?> LoadBitmapAsync(string url)
    {
        try
        {
            var stream = await _httpClient.GetStreamAsync(url);
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new(memoryStream);
        }
        catch
        {
            return null;
        }
    }
}