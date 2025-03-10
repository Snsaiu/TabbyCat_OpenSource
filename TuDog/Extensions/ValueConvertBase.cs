using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace TuDog.Extensions;

public abstract class ValueConvertBase : MarkupExtension, IValueConverter
{
    public virtual object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value;
    }

    public virtual object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}