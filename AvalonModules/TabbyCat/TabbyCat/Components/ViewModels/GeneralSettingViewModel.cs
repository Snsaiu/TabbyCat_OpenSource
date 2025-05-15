using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TabbyCat.Enums;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.ViewModels;
using TuDog.IocAttribute;
using ViewModelBase = TabbyCat.ViewModels.Bases.ViewModelBase;

namespace TabbyCat.Components.ViewModels;

[Register]
public sealed partial class GeneralSettingViewModel(ICloseWindowStateService closeWindowStateService,ITopMostService topMostService):ViewModelBase
{
    [ObservableProperty]
    private WindowCloseState closeState;

    [ObservableProperty]
    private bool topMost;

    protected override Task OnLoaded()
    {
        CloseState = closeWindowStateService.Get();
        this.TopMost = topMostService.Get();
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
}