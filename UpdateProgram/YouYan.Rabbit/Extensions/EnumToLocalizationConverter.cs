using System;
using System.Globalization;
using TuDog.Extensions;
using YouYan.Rabbit.Languages;

namespace YouYan.Rabbit.Extensions;

public sealed class EnumToLocalizationConverter : ValueConvertBase
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Enum @enum)
        {
            var name = LocalizationResourceManager.Instance[@enum.ToString()];
            // var name = Language.ResourceManager.GetString(@enum.ToString());
            return name;
        }

        return value.ToString();
    }
}