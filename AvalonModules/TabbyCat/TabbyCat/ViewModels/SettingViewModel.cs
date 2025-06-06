using CommunityToolkit.Mvvm.Input;
using TabbyCat.Components.ViewModels;
using TabbyCat.ViewModels.Bases;
using TuDog.Interfaces.Navigations;
using TuDog.Interfaces.RegionManagers;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public partial class SettingViewModel(IRegionManager regionManager, INavigationService navigationService)
    : ViewModelBase
{
    protected override Task OnLoaded()
    {
        regionManager.AddToRegion<GeneralSettingViewModel>("commonContainer");
        regionManager.AddToRegion<AiSettingViewModel>("aiContainer");
        regionManager.AddToRegion<PersonalizationViewModel>("personalizationContainer");
        regionManager.AddToRegion<MediaSettingViewModel>("mediaSettingContainer");
        regionManager.AddToRegion<ExperimentalFeaturesSettingViewModel>("experimentalFeaturesContainer");
        return Task.CompletedTask;

    }

    [RelayCommand]
    private Task ReturnPage()
    {
        return navigationService.PopAsync();
    }
}