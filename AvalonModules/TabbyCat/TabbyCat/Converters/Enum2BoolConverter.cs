using System.Globalization;
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
}