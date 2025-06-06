using System.Globalization;
using System.Reflection;
using Avalonia;
using TuDog.Extensions;

namespace TabbyCat.Converters;

public sealed class Enum2BoolConverter:ValueConvertBase
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {

        if(value is not Enum @enum || parameter is null)
            return false;
        return @enum.ToString() == parameter.ToString();
    }

    public override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue && boolValue && parameter != null && targetType.IsEnum)
            return Enum.Parse(targetType, parameter.ToString()!);

        return AvaloniaProperty.UnsetValue; // 不符合条件时保持不变
    }

}