using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using FluentAvalonia.UI.Controls;

namespace TabbyCat.Controls;

[TemplatePart(Name = PART_Add,Type = typeof(FontIcon))]
[TemplatePart(Name = PART_Delete,Type = typeof(FontIcon))]
public class ImageControl : TemplatedControl
{
    private const string PART_Delete = nameof(PART_Delete);
    private const string PART_Add = nameof(PART_Add);
    
    private FontIcon deleteIcon;
    private FontIcon addIcon;
    

    public static readonly StyledProperty<string> HeaderProperty = AvaloniaProperty.Register<ImageControl, string>(
        nameof(Header));

    public string Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly StyledProperty<bool> ShowHeaderProperty = AvaloniaProperty.Register<ImageControl, bool>(
        nameof(ShowHeader));

    public bool ShowHeader
    {
        get => GetValue(ShowHeaderProperty);
        set => SetValue(ShowHeaderProperty, value);
    }

    public static readonly StyledProperty<string> ImageSourceProperty = AvaloniaProperty.Register<ImageControl, string>(
        nameof(ImageSource));

    public string ImageSource
    {
        get => GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public static readonly StyledProperty<ICommand> CommandProperty = AvaloniaProperty.Register<ImageControl, ICommand>(
        nameof(Command));

    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }


    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
       addIcon = e.NameScope.Get<FontIcon>(PART_Add);
       deleteIcon = e.NameScope.Get<FontIcon>(PART_Delete);

       deleteIcon.PointerReleased += (_, _) =>
       {
           ImageSource = string.Empty;
       };

       addIcon.PointerReleased += (_, _) =>
       {
           Command?.Execute(null);
       };

       this.GetObservable(ImageSourceProperty).Subscribe(x =>
       {
            deleteIcon.IsVisible=!string.IsNullOrEmpty(ImageSource);
       });

    }
}