using CommunityToolkit.Mvvm.ComponentModel;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.ViewModels;
using TuDog.IocAttribute;
using ViewModelBase = TabbyCat.ViewModels.Bases.ViewModelBase;

namespace TabbyCat.Components.ViewModels;

[Register]
public sealed partial class ExperimentalFeaturesSettingViewModel(IUseMarkdownService useMarkdownService,IHotKeyStartProgramService hotKeyStartProgramService) : ViewModelBase
{
    [ObservableProperty] private bool useMarkdown;
    [ObservableProperty] private bool useHotkey;

    partial void OnUseMarkdownChanged(bool value)
    {
        useMarkdownService.Set(value);
    }

    partial void OnUseHotkeyChanged(bool value)
    {
        hotKeyStartProgramService.Set(value);
    }

    protected override Task OnLoaded()
    {
        UseMarkdown = useMarkdownService.Get();
        UseHotkey= hotKeyStartProgramService.Get();
        return base.OnLoaded();
    }
}