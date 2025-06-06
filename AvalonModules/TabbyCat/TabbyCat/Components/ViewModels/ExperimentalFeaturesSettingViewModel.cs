using CommunityToolkit.Mvvm.ComponentModel;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels;
using TuDog.IocAttribute;
using ViewModelBase = TabbyCat.ViewModels.Bases.ViewModelBase;

namespace TabbyCat.Components.ViewModels;

[Register]
public sealed partial class ExperimentalFeaturesSettingViewModel(IUseMarkdownService useMarkdownService,
    IHotKeyStartProgramService hotKeyStartProgramService,
    IShowQuickMenuService showQuickMenuService) : ViewModelBase
{
    [ObservableProperty] private bool useMarkdown;
    [ObservableProperty] private bool useHotkey;
    [ObservableProperty] private bool useCpShowQuickMenu;
    partial void OnUseMarkdownChanged(bool value)
    {
        useMarkdownService.Set(value);
    }

    partial void OnUseHotkeyChanged(bool value)
    {
        hotKeyStartProgramService.Set(value);
    }

    partial void OnUseCpShowQuickMenuChanged(bool value)
    {
        showQuickMenuService.Set(value);
    }

    protected override Task OnLoaded()
    {
        UseMarkdown = useMarkdownService.Get();
        UseHotkey= hotKeyStartProgramService.Get();
      
        UseCpShowQuickMenu = showQuickMenuService.Get();
            
        return base.OnLoaded();
    }
}