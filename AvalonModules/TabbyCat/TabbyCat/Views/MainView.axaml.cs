using Avalonia.Controls;
using Avalonia.Threading;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.ViewModels;
using TabbyCat.ViewModels.Bases;
using TuDog.Bootstrap;
using TuDog.Interfaces.RegionManagers;

namespace TabbyCat.Views
{
    public partial class MainView : UserControl
    {
        private IRegionManager _regionManager = TuDogApplication.ServiceProvider.GetRequiredService<IRegionManager>();

        private INavigationMenuItemService _navigationMenuItemService =
            TuDogApplication.ServiceProvider.GetRequiredService<INavigationMenuItemService>();

        public MainView()
        {
            InitializeComponent();
            navigationView.Loaded += (s, e) =>
            {
                navigationView.SelectedItem = navigationView.MenuItemsSource.ElementAt(0);
            };


        }


        private void NavigationView_OnSelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e)
        {
            if (e.IsSettingsSelected)
                _regionManager.AddToRegion<SettingViewModel>("navigationViewContainer");
            else if (e.SelectedItem is NavigationMenuItem { Content: not null } and var item)
            {
                item.IsSelected = true;
               
                var vm = _regionManager.AddToRegionReturnViewModel("navigationViewContainer", item.Content);
                if (vm is IMediaNavigation mediaNavigation && _navigationMenuItemService.Parameter is not null)
                {
                    mediaNavigation.NavigationAsync(_navigationMenuItemService.Parameter).ContinueWith(x =>
                    {
                        _navigationMenuItemService.Parameter = null;
                    });
                }
              
            }

        }
    }
}