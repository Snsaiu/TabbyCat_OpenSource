using System;
using System.Globalization;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using TuDog.Extensions;

namespace YouYan.Rabbit.Extensions;

public sealed class AppIconConverter:ValueConvertBase
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is AppName name)
        {
            return new Bitmap(AssetLoader.Open( new Uri($"avares://YouYan.Rabbit/Assets/{name.ToString()}.png")));
        }

        return null;
    }
}