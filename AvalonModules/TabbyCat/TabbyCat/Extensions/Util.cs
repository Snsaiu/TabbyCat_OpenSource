using System.IO;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;
using PixelFormat = Xabe.FFmpeg.PixelFormat;

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
            throw new Exception("无法读取图像文件");

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

    /// <summary>
    /// 获得视频某一帧作为图片
    /// </summary>
    /// <param name="videoPath">视频路径</param>
    /// <param name="snapshotTime">某一时刻作为缩略图</param>
    /// <param name="outputImagePath">图片输出全路径</param>
    /// <returns></returns>
    public static async Task<bool> GenerateThumbnail(string videoPath,
        TimeSpan snapshotTime, string outputImagePath)
    {
        await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official);
        var conversion = await FFmpeg.Conversions.FromSnippet.Snapshot(videoPath, outputImagePath, snapshotTime);
        await conversion.Start();
        return File.Exists(outputImagePath);
    }

    /// <summary>
    /// 将一张图片颜色取反，并保存到新的文件。
    /// </summary>
    /// <param name="inputPath">输入图片文件路径</param>
    /// <param name="outputPath">输出保存文件路径</param>
    public static void InvertImageColors(string inputPath, string outputPath)
    {
        if (!File.Exists(inputPath))
            throw new FileNotFoundException("输入文件不存在", inputPath);

        // 加载图片
        using var original = new Bitmap(inputPath);

        var pixelSize = original.PixelSize;
        var dpi = original.Dpi;
        var stride = pixelSize.Width * 4; // 每行像素字节数


        // 拷贝像素数据
        var pixelData = new byte[pixelSize.Height * stride];
        var pixelPtr = Marshal.AllocHGlobal(pixelData.Length);

        try
        {
            original.CopyPixels(
                new PixelRect(0, 0, pixelSize.Width, pixelSize.Height),
                pixelPtr,
                pixelData.Length,
                stride);

            // 把数据从 unmanaged memory 拷贝到 byte[]
            Marshal.Copy(pixelPtr, pixelData, 0, pixelData.Length);
        }
        finally
        {
            Marshal.FreeHGlobal(pixelPtr);
        }


        // 取反每个像素颜色
        for (var i = 0; i < pixelData.Length; i += 4)
        {
            // BGRA顺序
            pixelData[i + 0] = (byte)(255 - pixelData[i + 0]); // B
            pixelData[i + 1] = (byte)(255 - pixelData[i + 1]); // G
            pixelData[i + 2] = (byte)(255 - pixelData[i + 2]); // R
            // A（透明度）保持不变
        }

        // 把修改后的像素数据写到新的WriteableBitmap
        var wb = new WriteableBitmap(
            pixelSize,
            dpi,
            Avalonia.Platform.PixelFormat.Bgra8888,
            AlphaFormat.Premul);

        using (var framebuffer = wb.Lock())
        {
            unsafe
            {
                fixed (byte* src = pixelData)
                {
                    Buffer.MemoryCopy(
                        src,
                        (void*)framebuffer.Address,
                        pixelData.Length,
                        pixelData.Length);
                }
            }
        }

        // 保存新图像
        using var fs = File.Create(outputPath);
        wb.Save(fs);
    }
}