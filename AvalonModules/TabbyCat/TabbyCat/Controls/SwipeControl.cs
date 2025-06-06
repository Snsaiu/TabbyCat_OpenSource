using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Metadata;

namespace TabbyCat.Controls;

[TemplatePart(Name = "PART_Border", Type = typeof(Border))]
[TemplatePart(Name = "PART_Grid", Type = typeof(Grid))]
public sealed class SwipeControl:TemplatedControl
{
    
    private Border _partBorder;
    
    private Grid _grid;
    
    private const string PART_Border= "PART_Border";

    private const string PART_Grid = "PART_Grid";

    public static readonly StyledProperty<object?> SwipeContentProperty = AvaloniaProperty.Register<SwipeControl, object?>(
        nameof(SwipeContent));

    public object? SwipeContent
    {
        get => GetValue(SwipeContentProperty);
        set => SetValue(SwipeContentProperty, value);
    }

    public static readonly StyledProperty<object?> ContentProperty = AvaloniaProperty.Register<SwipeControl, object?>(
        nameof(Content));

    [Content]
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _partBorder = e.NameScope.Get<Border>(PART_Border);
        _grid = e.NameScope.Get<Grid>(PART_Grid);
        
        _partBorder.GestureRecognizers.Add(new PullGestureRecognizer(PullDirection.RightToLeft));
        _partBorder.AddHandler(Gestures.PullGestureEndedEvent,borderPullGestureEnded);
        
        _grid.GestureRecognizers.Add(new PullGestureRecognizer(PullDirection.LeftToRight));
        _grid.AddHandler(Gestures.PullGestureEndedEvent,stackPanelPullGestureEnded);

    }

    private void stackPanelPullGestureEnded(object? sender, PullGestureEndedEventArgs e)
    {
        if(e.PullDirection != PullDirection.LeftToRight)
            return;
        
        _grid.IsVisible = false;
    }

    private void borderPullGestureEnded(object? sender, PullGestureEndedEventArgs e)
    {
        if(e.PullDirection!=PullDirection.RightToLeft)
            return;
        
        _grid.IsVisible = true;
    }
}