using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.IServices;
using TabbyCat.Models;
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

        var contact = NavigationMenuItem.Create(AppResources.Contact, IconFontProvider.Contact,
            typeof(ContactViewModel),
            AiMediaWorkType.Contact);
        
        _tailItems.Add(contact);
        
        navigationMenuItems.Add(contact);

        var aichat = NavigationMenuItem.Create(
            AppResources.Chat,
            IconFontProvider.TxtChat,
            typeof(ChatViewModel), AiMediaWorkType.AiChat
        );
        
        _tailItems.Add(aichat);
        
        navigationMenuItems.Add(aichat);
        
        navigationMenuItems.Add(new()
        {
            Header = AppResources.ImageProcessing,
            Icon = IconFontProvider.ImageEdit,
            Children =InitImageProcessMenuItems()
          
        });


        var textToVideo = NavigationMenuItem.Create(
            AppResources.TextToVideo,
            IconFontProvider.TextToVideo,
            typeof(TextToVideoViewModel),
            AiMediaWorkType.TextToVideo
        );
        _tailItems.Add(textToVideo);
        
        navigationMenuItems.Add(new()
        {
            Header = AppResources.VideoProcessing,
            Icon = IconFontProvider.VideoEdit,
            Children =
            [
             textToVideo  
            ]
        });

        MenuItems = navigationMenuItems;
    }
    
    
    private IEnumerable<NavigationMenuItem> InitImageProcessMenuItems()
    {
        IEnumerable<NavigationMenuItem> items =
        [
            NavigationMenuItem.Create(
                AppResources.TextToImage,
                IconFontProvider.WenShengTu,
                typeof(TextToImageViewModel),
                AiMediaWorkType.TextToImage
            ),
            NavigationMenuItem.Create(AppResources.CommandsEdit, IconFontProvider.CommandEditImage,
                typeof(CommandEditImageViewModel), AiMediaWorkType.CommandEditImage),
            NavigationMenuItem.Create(AppResources.PartialRepaint, IconFontProvider.PartialDrawImage,
                typeof(PartialRepaintImageViewModel), AiMediaWorkType.PartialRepaintImage),
            NavigationMenuItem.Create(AppResources.ExpandImage, IconFontProvider.ExpandImage,
                typeof(ExpandImageViewModel), AiMediaWorkType.ExpandImage),
            NavigationMenuItem.Create(AppResources.RemoveWatermark, IconFontProvider.RemoveMark,
                typeof(RemoveWatermarkViewModel), AiMediaWorkType.RemoveWatermark),
            NavigationMenuItem.Create(AppResources.ImageSuperResolution, IconFontProvider.SuperResoluntion,
                typeof(ImageSuperResolutionViewModel), AiMediaWorkType.ImageSuperResolution),
            NavigationMenuItem.Create(AppResources.ImageColorization, IconFontProvider.Colorize,
                typeof(ImageColorizationViewModel), AiMediaWorkType.ImageColorization),
            NavigationMenuItem.Create(AppResources.GraffitiPainting, IconFontProvider.Doodle,
                typeof(GraffitiPaintingViewModel), AiMediaWorkType.GraffitiPainting),
            NavigationMenuItem.Create(AppResources.AvatarStylization,IconFontProvider.AvatarStylization,typeof(AvatarStylizationViewModel),AiMediaWorkType.AvatarStylization)
        ];
        _tailItems.AddRange(items);
        return items;
    }

    public IEnumerable<NavigationMenuItem> MenuItems { get; private set; }
    
    private List<NavigationMenuItem> _tailItems = [];

    public Action<NavigationMenuItem> SelectMenuItemAction { get; set; }
    public object? Parameter { get; set; }

    public Task NavigationAsync(AiMediaWorkType aiMediaWorkType,object? parameter)
    {
        if (_tailItems.FirstOrDefault(x => x.MediaWorkType == aiMediaWorkType) is not null and NavigationMenuItem item)
        { 
            Parameter=parameter;
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