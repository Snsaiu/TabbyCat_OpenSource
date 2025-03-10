using CommunityToolkit.Mvvm.ComponentModel;

using System.Collections.ObjectModel;

using TabbyCat.Models;
using TabbyCat.Shared.Languages;

using TuDog.IocAttribute;

namespace TabbyCat.ViewModels
{
    [Register]
    public partial class MainViewModel : ViewModelBase
    {

        [ObservableProperty] private ObservableCollection<NavigationMenuItem> menuItems = [];

        [ObservableProperty] private NavigationMenuItem? selectMenuItem;

        protected override Task OnLoaded()
        {
            CreateMenuItems();
            return base.OnLoaded();
        }

        private void CreateMenuItems()
        {
            MenuItems.Add(new()
            {
                Header = AppResources.Chat,
                Content = typeof(ChatViewModel),
                Icon = IconFontProvider.TxtChat

            });
            MenuItems.Add(new()
            {
                Header = AppResources.CutOutImage,
                Content = typeof(CutoutViewModel),
                Icon = IconFontProvider.Kotu


            });
            MenuItems.Add(new()
            {
                Header = AppResources.TextToImage,
                Content = typeof(TextToImageViewModel),
                Icon = IconFontProvider.WenShengTu


            });
            // MenuItems.Add(new()
            // {
            //     Header = "图生图",
            //     Content = typeof(ImageToImageViewModel),
            //     Icon = IconFontProvider.TuShengTu
            //
            //
            // });
            SelectMenuItem = MenuItems.First();
        }

        partial void OnSelectMenuItemChanged(NavigationMenuItem value)
        {
            // this._regionManager.AddToRegion("navigationViewContainer", value.Content);
        }


    }
}