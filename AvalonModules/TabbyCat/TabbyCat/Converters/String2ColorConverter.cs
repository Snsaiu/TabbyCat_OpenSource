using System.Globalization;
using Avalonia.Media;
using TuDog.Extensions;

namespace TabbyCat.Converters;

public sealed class String2ColorConverter : ValueConvertBase
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
            throw new NullReferenceException();

        var hash = value.ToString().GetHashCode();

        // 确保哈希值为正数
        hash = Math.Abs(hash);

        // 从哈希值中提取 RGB 分量（可以调整位运算方式）
        int r = (hash & 0xFF0000) >> 16;
        int g = (hash & 0x00FF00) >> 8;
        int b = (hash & 0x0000FF);

        // 确保颜色不太暗（可选）
        r = Math.Max(r, 50);
        g = Math.Max(g, 50);
        b = Math.Max(b, 50);
        return new SolidColorBrush(new Color(255, (byte)r, (byte)g, (byte)b));
    }
}