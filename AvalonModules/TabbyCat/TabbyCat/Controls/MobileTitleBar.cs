using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;

namespace TabbyCat.Controls;

public class MobileTitleBar : TemplatedControl
{
    public static readonly StyledProperty<bool> ShowLeftArrowProperty = AvaloniaProperty.Register<MobileTitleBar, bool>(
        nameof(ShowLeftArrow));

    public bool ShowLeftArrow
    {
        get => GetValue(ShowLeftArrowProperty);
        set => SetValue(ShowLeftArrowProperty, value);
    }

    public static readonly StyledProperty<ICommand> LeftArrowCommandProperty = AvaloniaProperty.Register<MobileTitleBar, ICommand>(
        nameof(LeftArrowCommand));

    public ICommand LeftArrowCommand
    {
        get => GetValue(LeftArrowCommandProperty);
        set => SetValue(LeftArrowCommandProperty, value);
    }

  
    public static readonly StyledProperty<object?> RightContentProperty = AvaloniaProperty.Register<MobileTitleBar, object?>(
        nameof(RightContent));
    
    public object? RightContent
    {
        get => GetValue(RightContentProperty);
        set => SetValue(RightContentProperty, value);
    }

    public static readonly StyledProperty<object?> TitleProperty = AvaloniaProperty.Register<MobileTitleBar, object?>(
        nameof(Title));
    
    public object? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
}