using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Shared;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels;
using TabbyCat.ViewModels.Bases;
using TuDog.Bootstrap;
using TuDog.IocAttribute;

namespace TabbyCat.Services;

[Register<INavigationMenuItemService>(ServiceLifetime.Singleton)]
public sealed partial class NavigationMenuItemService : ModelBase, INavigationMenuItemService
{
    public NavigationMenuItemService()
    {
        var navigationMenuItems = new List<NavigationMenuItem>();

        var contact = NavigationMenuItem.Create("Contact", IconFontProvider.Contact,
            typeof(ContactViewModel),
            AiMediaWorkType.Contact);

        _tailItems.Add(contact);

        navigationMenuItems.Add(contact);

        var aichat = NavigationMenuItem.Create(
            "Chat",
            IconFontProvider.TxtChat,
            typeof(ChatViewModel), AiMediaWorkType.AiChat
        );

        _tailItems.Add(aichat);

        navigationMenuItems.Add(aichat);

        navigationMenuItems.Add(
            NavigationMenuItem.Create("ImageProcessing", IconFontProvider.ImageEdit, InitImageProcessMenuItems()));


        var textToVideo = NavigationMenuItem.Create(
            "TextToVideo",
            IconFontProvider.TextToVideo,
            typeof(TextToVideoViewModel),
            AiMediaWorkType.TextToVideo
        );
        _tailItems.Add(textToVideo);

        navigationMenuItems.Add(NavigationMenuItem.Create
        (
            "VideoProcessing",
            IconFontProvider.VideoEdit,
            [textToVideo]
        ));

        MenuItems = navigationMenuItems;
    }


    private IEnumerable<NavigationMenuItem> InitImageProcessMenuItems()
    {
        IEnumerable<NavigationMenuItem> items =
        [
            NavigationMenuItem.Create(
                "TextToImage",
                IconFontProvider.WenShengTu,
                typeof(TextToImageViewModel),
                AiMediaWorkType.TextToImage
            ),
            NavigationMenuItem.Create("CommandsEdit", IconFontProvider.CommandEditImage,
                typeof(CommandEditImageViewModel), AiMediaWorkType.CommandEditImage),
            NavigationMenuItem.Create("PartialRepaint", IconFontProvider.PartialDrawImage,
                typeof(PartialRepaintImageViewModel), AiMediaWorkType.PartialRepaintImage),
            NavigationMenuItem.Create("ExpandImage", IconFontProvider.ExpandImage,
                typeof(ExpandImageViewModel), AiMediaWorkType.ExpandImage),
            NavigationMenuItem.Create("RemoveWatermark", IconFontProvider.RemoveMark,
                typeof(RemoveWatermarkViewModel), AiMediaWorkType.RemoveWatermark),
            NavigationMenuItem.Create("ImageSuperResolution", IconFontProvider.SuperResoluntion,
                typeof(ImageSuperResolutionViewModel), AiMediaWorkType.ImageSuperResolution),
            NavigationMenuItem.Create("ImageColorization", IconFontProvider.Colorize,
                typeof(ImageColorizationViewModel), AiMediaWorkType.ImageColorization),
            NavigationMenuItem.Create("GraffitiPainting", IconFontProvider.Doodle,
                typeof(GraffitiPaintingViewModel), AiMediaWorkType.GraffitiPainting),
            NavigationMenuItem.Create("AvatarStylization", IconFontProvider.AvatarStylization,
                typeof(AvatarStylizationViewModel), AiMediaWorkType.AvatarStylization)
        ];
        _tailItems.AddRange(items);
        return items;
    }

    public IEnumerable<NavigationMenuItem> MenuItems { get; private set; }

    private List<NavigationMenuItem> _tailItems = [];

    public Action<NavigationMenuItem> SelectMenuItemAction { get; set; }
    public object? Parameter { get; set; }

    public Task NavigationAsync(AiMediaWorkType aiMediaWorkType, object? parameter)
    {
        if (_tailItems.FirstOrDefault(x => x.MediaWorkType == aiMediaWorkType) is not null and NavigationMenuItem item)
        {
            Parameter = parameter;
            SelectMenuItem = null;
            SelectMenuItem = item;
            return Task.CompletedTask;
        }
        else
        {
            throw new ArgumentNullException();
        }
    }

    [ObservableProperty] private NavigationMenuItem _selectMenuItem;

    partial void OnSelectMenuItemChanged(NavigationMenuItem value)
    {
        SelectMenuItemAction?.Invoke(value);
    }
}