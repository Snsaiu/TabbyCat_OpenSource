using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.IServices;
using TabbyCat.Shared;
using TuDog.Bootstrap;
using TuDog.Interfaces.IDialogServers;
using TuDog.Interfaces.MessageBarService;
using TuDog.Interfaces.RegionManagers;

namespace TabbyCat.ViewModels.Bases
{
    public abstract partial class ViewModelBase : TuDogViewModelBase
    {
        protected IDialogServer DialogServer { get; } =
            TuDogApplication.ServiceProvider.GetRequiredService<IDialogServer>();
        
        protected LocalizationResourceManager ResourceManager { get; } = LocalizationResourceManager.Instance;

        protected IRegionManager RegionManager { get; } =
            TuDogApplication.ServiceProvider.GetRequiredService<IRegionManager>();
        
        protected IMessageBarService MessageBarService { get; } =TuDogApplication.ServiceProvider.GetRequiredService<IMessageBarService>();

        public INavigationMenuItemService NavigationMenuItemService { get; } =
            TuDogApplication.ServiceProvider.GetRequiredService<INavigationMenuItemService>();

    }
}