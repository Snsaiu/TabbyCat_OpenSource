using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TabbyCat.Models;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels.Bases;
using TuDog.Extensions;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels
{
    [Register]
    public partial class MainViewModel : ViewModelBase
    {
        protected override Task OnLoaded()
        {
            //  NavigationMenuItemService.SelectMenuItem = NavigationMenuItemService.MenuItems.ElementAt(1);
            return base.OnLoaded();
        }


    }
}