using System;
using System.Globalization;
using TuDog.Extensions;

namespace YouYan.Rabbit.Extensions;

public sealed class WhatNew2BoolConverter : ValueConvertBase
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not AppStatus status)
            return false;
        return status is AppStatus.Waiting or AppStatus.Downloading or AppStatus.Installing or AppStatus.NeedUpdate;
    }
}