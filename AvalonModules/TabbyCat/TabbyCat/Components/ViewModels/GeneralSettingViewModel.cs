using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TabbyCat.Enums;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.ViewModels;
using TabbyCat.Views;
using TuDog.IocAttribute;
using ViewModelBase = TabbyCat.ViewModels.Bases.ViewModelBase;

namespace TabbyCat.Components.ViewModels;

[Register]
public sealed partial class GeneralSettingViewModel(
    ICloseWindowStateService closeWindowStateService,
    ITopMostService topMostService,
    FloatingFrameWindow floatingFrameWindow,
    IShowFloatingFrameService showFloatingFrameService) : ViewModelBase
{
    [ObservableProperty] private WindowCloseState closeState;

    [ObservableProperty] private bool topMost;

    [ObservableProperty] private bool showFloatingFrame;


    protected override Task OnLoaded()
    {
        CloseState = closeWindowStateService.Get();
        TopMost = topMostService.Get();
        ShowFloatingFrame = showFloatingFrameService.Get();
        return base.OnLoaded();
    }

    partial void OnCloseStateChanged(WindowCloseState value)
    {
        closeWindowStateService.Set(value);
    }

    partial void OnTopMostChanged(bool value)
    {
        topMostService.Set(value);
    }

    partial void OnShowFloatingFrameChanged(bool value)
    {
        showFloatingFrameService.Set(value);
        if (value)
            floatingFrameWindow.Show();
        else
            floatingFrameWindow.Hide();
    }
}