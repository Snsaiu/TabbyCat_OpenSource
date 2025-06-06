using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;

namespace TabbyCat.Controls;

public  class BoxControl:TemplatedControl
{
    public static readonly StyledProperty<object?> ContentProperty = AvaloniaProperty.Register<BoxControl, object?>(
        nameof(Content));

    [Content]
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly StyledProperty<object?> AttachContentProperty = AvaloniaProperty.Register<BoxControl, object?>(
        nameof(AttachContent));

    public object? AttachContent
    {
        get => GetValue(AttachContentProperty);
        set => SetValue(AttachContentProperty, value);
    }
    
}


public sealed class DeleteBoxControl : BoxControl
{
 
    
    public static readonly StyledProperty<ICommand> DeleteCommandProperty = AvaloniaProperty.Register<DeleteBoxControl, ICommand>(
        nameof(DeleteCommand));

    public ICommand DeleteCommand
    {
        get => GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }
    
    public static readonly StyledProperty<object?> DeleteCommandParameterProperty = AvaloniaProperty.Register<DeleteBoxControl, object?>(
        nameof(DeleteCommandParameter));
    
    public object? DeleteCommandParameter
    {
        get => GetValue(DeleteCommandParameterProperty);
        set => SetValue(DeleteCommandParameterProperty, value);
    }
    
}