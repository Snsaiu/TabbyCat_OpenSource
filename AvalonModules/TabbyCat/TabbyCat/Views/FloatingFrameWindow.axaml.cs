using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.Extensions;
using TabbyCat.ViewModels;
using TuDog.Bootstrap;
using TuDog.Interfaces.RegionManagers;

namespace TabbyCat.Views;

public partial class FloatingFrameWindow : Window
{
    private IRegionManager regionManager = TuDogApplication.ServiceProvider.GetRequiredService<IRegionManager>();

    public FloatingFrameWindow()
    {
        InitializeComponent();
        regionManager.AddToRegion<FloatingFrameViewModel>("floatingFrameContainer");
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        if (OperatingSystem.IsWindows()) Win32Utils.HideFromAltTab(this);
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed) BeginMoveDrag(e);
    }
}