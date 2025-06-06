using Avalonia;

namespace TabbyCat.Extensions;

public sealed class AttachContent : AvaloniaObject
{
    public static readonly AttachedProperty<object> AttachContentProperty =
        AvaloniaProperty.RegisterAttached<AttachContent, AvaloniaObject, object>("AttachContent");

    public static void SetAttachContent(AvaloniaObject obj, object value)
    {
        obj.SetValue(AttachContentProperty, value);
    }

    public static object GetAttachContent(AvaloniaObject obj)
    {
        return obj.GetValue(AttachContentProperty);
    }
}