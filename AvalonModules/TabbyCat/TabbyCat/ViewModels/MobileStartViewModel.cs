using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TabbyCat.Enums;
using TabbyCat.ViewModels.Bases;
using TuDog.Interfaces.RegionManagers;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public partial class MobileStartViewModel(IRegionManager regionManager) : ViewModelBase
{
    private MobileNavigationItem _backMobileNavigationItem = MobileNavigationItem.Chat;
    private bool isLoaded = false;


    [RelayCommand]
    private Task NavigationTo(MobileNavigationItem value)
    {
        if (_backMobileNavigationItem == value)
            return Task.CompletedTask;

        switch (value)
        {
            case MobileNavigationItem.Chat:
                regionManager.AddToRegion<MobileChatListViewModel>("mobileContainer");
                break;
            case MobileNavigationItem.Contact:
                regionManager.AddToRegion<MobileContactViewModel>("mobileContainer");
                break;
            case MobileNavigationItem.Favorites:
                regionManager.AddToRegion<MobileFavouriteViewModel>("mobileContainer");
                break;
            case MobileNavigationItem.Mine:
                regionManager.AddToRegion<MobilePersonViewModel>("mobileContainer");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(value), value, null);
        }

        _backMobileNavigationItem = value;
        return Task.CompletedTask;

    }




    protected override Task OnLoaded()
    {
        if (isLoaded)
            return Task.CompletedTask;

        regionManager.AddToRegion<MobileChatListViewModel>("mobileContainer");
        isLoaded = true;
        return Task.CompletedTask;
    }
}