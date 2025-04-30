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
                Header = AppResources.ImageProcessing,
                Icon = IconFontProvider.ImageEdit,
                Children =
                [
                    new()
                    {
                        Header = AppResources.TextToImage,
                        Icon = IconFontProvider.WenShengTu,
                        Content = typeof(TextToImageViewModel)
                    },
                    NavigationMenuItem.Create(AppResources.CommandsEdit, IconFontProvider.CommandEditImage,
                        typeof(CommandEditImageViewModel)),
                    NavigationMenuItem.Create(AppResources.PartialRepaint, IconFontProvider.PartialDrawImage,
                        typeof(PartialRepaintImageViewModel)),
                    NavigationMenuItem.Create(AppResources.ExpandImage, IconFontProvider.ExpandImage,
                        typeof(ExpandImageViewModel)),
                    NavigationMenuItem.Create(AppResources.RemoveWatermark, IconFontProvider.RemoveMark,
                        typeof(RemoveWatermarkViewModel)),
                    NavigationMenuItem.Create(AppResources.ImageSuperResolution, IconFontProvider.SuperResoluntion,
                        typeof(ImageSuperResolutionViewModel)),
                    NavigationMenuItem.Create(AppResources.ImageColorization, IconFontProvider.Colorize,
                        typeof(ImageColorizationViewModel)),
                    NavigationMenuItem.Create(AppResources.GraffitiPainting, IconFontProvider.Doodle,
                        typeof(GraffitiPaintingViewModel)),
                    NavigationMenuItem.Create(AppResources.ImageErasureAndCompletion, IconFontProvider.EraseEdit,
                        typeof(ImageErasureAndCompletionViewModel))

                ]
            });

            MenuItems.Add(new()
            {
                Header = AppResources.VideoProcessing,
                Icon = IconFontProvider.VideoEdit,
                Children =
                [
                    new()
                    {
                        Header = AppResources.TextToVideo,
                        Icon = IconFontProvider.TextToVideo,
                        Content = typeof(TextToVideoViewModel)
                    }
                ]
            });

            SelectMenuItem = MenuItems.First();
        }
    }
}