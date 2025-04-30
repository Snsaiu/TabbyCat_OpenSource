using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.Models;
using TabbyCat.ViewModels;
using TuDog.Bootstrap;
using TuDog.Interfaces.RegionManagers;

namespace TabbyCat.Views
{
    public partial class MainView : UserControl
    {
        private IRegionManager _regionManager = TuDogApplication.ServiceProvider.GetRequiredService<IRegionManager>();
        public MainView()
        {
            InitializeComponent();

        }


        private void NavigationView_OnSelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e)
        {
            if (e.IsSettingsSelected)
            {
                _regionManager.AddToRegion<SettingViewModel>("navigationViewContainer");
            }
            else if (e.SelectedItem is NavigationMenuItem { Content: not null and Type content })
            {
                _regionManager.AddToRegion("navigationViewContainer", content);
            }
        }
    }
}