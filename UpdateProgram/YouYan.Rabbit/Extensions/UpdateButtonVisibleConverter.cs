using System;
using System.Globalization;
using TuDog.Extensions;
using YouYan.Rabbit.Models;

namespace YouYan.Rabbit.Extensions;

public sealed class UpdateButtonVisibleConverter : ValueConvertBase
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is AppListItemModel model)
            return model.LatestVersion != model.Version;
        else
            return null;
    }
}