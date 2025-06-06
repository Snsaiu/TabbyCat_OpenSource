using System.Globalization;

namespace TuDog.Extensions.Converters;

public sealed class EnumToRadioButtonCheckedStateConverter : ValueConvertBase
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        if (parameter is not Enum valueAsEnum)
            throw new ArgumentException();

        if (value is Enum enumValue)
        {
            return enumValue.Equals(valueAsEnum);
        }
        else
        {
            throw new ArgumentException();
        }
    }

    public override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return parameter;
    }
}