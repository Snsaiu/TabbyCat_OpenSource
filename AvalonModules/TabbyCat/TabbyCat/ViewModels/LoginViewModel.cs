using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System.Threading.Tasks;

using TuDog.Interfaces.RegionManagers;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public partial class LoginViewModel:ViewModelBase
{
    private readonly IRegionManager _regionManager;

    public LoginViewModel(IRegionManager regionManager)
    {
        _regionManager = regionManager;
    }
    
    [RelayCommand]
    private Task Login()
    {
     this._regionManager.AddToRegion<MainViewModel>("mainContainer");  
        return Task.CompletedTask;
    }

    protected override Task OnLoaded()
    {
        return base.OnLoaded();
    }
}