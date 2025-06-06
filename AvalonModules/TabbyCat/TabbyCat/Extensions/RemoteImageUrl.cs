using System.IO;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media.Imaging;

namespace TabbyCat.Extensions;

public sealed class RemoteImageUrl : AvaloniaObject
{
    static RemoteImageUrl()
    {
        UrlProperty.Changed.AddClassHandler<Image>(OnUrlChanged);
    }

    private static HttpClient _httpClient = new();

    private static void OnUrlChanged(Image arg1, AvaloniaPropertyChangedEventArgs arg2)
    {
        if (arg2.NewValue is string url)
            Task<Bitmap?>.Run(async () => await LoadBitmapAsync(url)).ContinueWith(x =>
            {
                if (x.Result is not null)
                    arg1.Source = x.Result;
            }, TaskScheduler.FromCurrentSynchronizationContext());
    }

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


    public static readonly AttachedProperty<string> UrlProperty =
        AvaloniaProperty.RegisterAttached<RemoteImageUrl, Image, string>("Url", defaultBindingMode: BindingMode.OneWay);


    public static void SetUrl(Image obj, string value)
    {
        obj.SetValue(UrlProperty, value);
    }

    public static string GetUrl(Image obj)
    {
        return obj.GetValue(UrlProperty);
    }
}