using System.Globalization;
using TabbyCat.Shared.Enums;
using TuDog.Extensions;

namespace TabbyCat.Converters;

public sealed class Role2BoolValueConverter:ValueConvertBase
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Role role)
        {
            return false;
        }

        return role == Role.User;
    }
}