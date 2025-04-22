using System.IO;
using SkiaSharp;

namespace TabbyCat.Extensions;

public static class Util
{
    public static string ImagePathToBase64(string imagePath, int maxWidth = 400, int maxHeight = 300,
        SKEncodedImageFormat format = SKEncodedImageFormat.Jpeg, int quality = 80)
    {
        if (!File.Exists(imagePath)) throw new FileNotFoundException("Image file could not be found", imagePath);
        using var stream = File.OpenRead(imagePath);
        using var original = SKBitmap.Decode(stream);

        if (original == null)
            throw new("无法读取图像文件");

        // 计算等比缩放尺寸
        var ratioX = (float)maxWidth / original.Width;
        var ratioY = (float)maxHeight / original.Height;
        var ratio = Math.Min(ratioX, ratioY);

        var newWidth = (int)(original.Width * ratio);
        var newHeight = (int)(original.Height * ratio);

        using var resized = original
            .Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.High);

        using var image = SKImage.FromBitmap(resized);
        using var outputStream = new MemoryStream();
        image.Encode(format, quality).SaveTo(outputStream);

        // 转换为 Base64
        return Convert.ToBase64String(outputStream.ToArray());
    }
}