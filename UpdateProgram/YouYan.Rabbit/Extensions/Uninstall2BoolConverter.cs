using System;
using System.Globalization;
using TuDog.Extensions;

namespace YouYan.Rabbit.Extensions;

public sealed class Uninstall2BoolConverter : ValueConvertBase
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not AppStatus status)
            return false;
        return status is AppStatus.Installed or AppStatus.NeedUpdate or AppStatus.Failed;
    }
}