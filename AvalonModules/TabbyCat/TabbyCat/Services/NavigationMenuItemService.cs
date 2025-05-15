using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels;
using TuDog.Bootstrap;
using TuDog.IocAttribute;

namespace TabbyCat.Services;

[Register<INavigationMenuItemService>(ServiceLifetime.Singleton)]
public sealed partial class NavigationMenuItemService : ModelBase, INavigationMenuItemService
{
    public NavigationMenuItemService()
    {
        var navigationMenuItems = new List<NavigationMenuItem>();
            navigationMenuItems.Add(new()
            {
                Header = AppResources.Contact,
                Icon = IconFontProvider.Contact,
                Content = typeof(ContactViewModel)
            });


            navigationMenuItems.Add(new()
            {
                Header = AppResources.Chat,
                Icon = IconFontProvider.TxtChat,
                Content = typeof(ChatViewModel)
            });

            navigationMenuItems.Add(new()
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

            navigationMenuItems.Add(new()
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

            MenuItems = navigationMenuItems;
    }

    public IEnumerable<NavigationMenuItem> MenuItems { get; private set; }

    public Action<NavigationMenuItem> SelectMenuItemAction { get; set; }

    [ObservableProperty] private NavigationMenuItem _selectMenuItem;

    partial void OnSelectMenuItemChanged(NavigationMenuItem value)
    {
        SelectMenuItemAction?.Invoke(value);
    }
}