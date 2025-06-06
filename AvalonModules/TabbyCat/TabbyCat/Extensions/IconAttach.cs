using Avalonia;
using Avalonia.Controls;

namespace TabbyCat.Extensions;

public class IconAttach : AvaloniaObject
{
    public static readonly AttachedProperty<string> IconProperty =
        AvaloniaProperty.RegisterAttached<IconAttach, ContentControl, string>("Icon", inherits: true);

    public static string GetIcon(ContentControl obj)
    {
        return obj.GetValue(IconProperty);
    }

    public static void SetIcon(ContentControl obj, string value)
    {
        obj.SetValue(IconProperty, value);
    }
}