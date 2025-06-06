using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace TabbyCat.Controls;

public class NavigationButton : RadioButton
{
    /// <summary>
    /// 图标
    /// </summary>
    public static readonly StyledProperty<string> IconProperty = AvaloniaProperty.Register<NavigationButton, string>(
        nameof(Icon));

    public string Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly StyledProperty<string> SelectedIconProperty = AvaloniaProperty.Register<NavigationButton, string>(
        nameof(SelectedIcon));

    public string SelectedIcon
    {
        get => GetValue(SelectedIconProperty);
        set => SetValue(SelectedIconProperty, value);
    }

   
}