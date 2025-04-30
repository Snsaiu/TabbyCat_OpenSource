using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TabbyCat.Components.ViewModels;
using TabbyCat.Controls;

namespace TabbyCat.Components.Views;

public partial class DrawMaskView : UserControl
{
    private ImageDrawControl _imageDrawControl;

    private DrawMaskViewModel _vm;
    public DrawMaskView()
    {
        InitializeComponent();
        _imageDrawControl = this.FindControl<ImageDrawControl>("imageDraw");


        this.Loaded += (_, _) =>
        {
            _vm = this.DataContext as DrawMaskViewModel;
            _vm.View = _imageDrawControl;
        };
    }
}