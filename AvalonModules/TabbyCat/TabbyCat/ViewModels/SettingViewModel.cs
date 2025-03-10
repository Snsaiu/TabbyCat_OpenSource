using TabbyCat.Components.ViewModels;

using TuDog.Interfaces.RegionManagers;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public partial class SettingViewModel(IRegionManager regionManager) : ViewModelBase
{
    protected override Task OnLoaded()
    {
        regionManager.AddToRegion<GeneralSettingViewModel>("commonContainer");
        regionManager.AddToRegion<AiSettingViewModel>("aiContainer");
        regionManager.AddToRegion<PersonalizationViewModel>("personalizationContainer");
        regionManager.AddToRegion<RunningHubSettingViewModel>("runningHubContainer");
        regionManager.AddToRegion<ExperimentalFeaturesSettingViewModel>("experimentalFeaturesContainer");
        return Task.CompletedTask;

    }
}