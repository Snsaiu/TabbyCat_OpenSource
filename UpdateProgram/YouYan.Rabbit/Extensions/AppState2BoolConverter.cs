using System;
using System.Globalization;
using TuDog.Extensions;

namespace YouYan.Rabbit.Extensions;

public sealed class AppState2BoolConverter : ValueConvertBase
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is AppStatus v) return v.ToString() == parameter?.ToString();

        return false;
    }
}