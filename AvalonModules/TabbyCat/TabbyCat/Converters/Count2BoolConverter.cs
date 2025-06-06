using System.Globalization;
using Avalonia;
using NCalc;
using TuDog.Extensions;

namespace TabbyCat.Converters;

public sealed class Count2BoolConverter:ValueConvertBase
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int and var v && parameter is string and var p)
        {
            var expr = new Expression($"{v}{p}");
            return  expr.Evaluate();
        }

        return AvaloniaProperty.UnsetValue;
    }
}