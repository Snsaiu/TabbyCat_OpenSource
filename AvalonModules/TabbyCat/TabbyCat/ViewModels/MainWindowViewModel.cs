using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TuDog.Bootstrap;
using TuDog.Interfaces.RegionManagers;

namespace TabbyCat.ViewModels;

public partial class MainWindowViewModel:ViewModelBase
{

    private IRegionManager _regionManager { get; }=TuDogApplication.ServiceProvider.GetRequiredService<IRegionManager>();

    protected override Task OnLoaded()
    {
        _regionManager.AddToRegion<MainViewModel>("mainContainer");
        return base.OnLoaded();
    }
}