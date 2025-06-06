using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TuDog.Bootstrap;
using TuDog.Interfaces.MessageBarService;
using TuDog.Interfaces.RegionManagers;
using TuDog.IocAttribute;
using YouYan.Rabbit.Extensions;
using YouYan.Rabbit.IServices.LocalConfigs;
using YouYan.Rabbit.Languages;

namespace YouYan.Rabbit.ViewModels;

[Register]
public sealed partial class SettingViewModel(
    IRegionManager regionManager,
    IAutoStartService autoStartService,
    ILanguageService languageService,
    IMessageBarService messageBarService)
    : ViewModelBase
{
    [ObservableProperty] private bool autoStart = false;

    [ObservableProperty] private string _selectedLanguage = string.Empty;

    [ObservableProperty] private ObservableCollection<KeyValuePair<string, string>> _languages = [];

    [RelayCommand]
    private Task ReturnHome()
    {
        regionManager.AddToRegion<AppListViewModel>("container");
        return Task.CompletedTask;
    }

    protected override Task OnLoaded()
    {
        AutoStart = autoStartService.Get();
        InitLanguages();
        return Task.CompletedTask;
    }


    partial void OnAutoStartChanged(bool value)
    {
        if (value == autoStartService.Get())
            return;

        if (OperatingSystem.IsWindows())
        {
            AutoStartHelper.WindowsSetAutoStart(value);
        }
        else
        {
            var appname = "com.youyan.rabbit";
            
            var homeDir = Environment.GetEnvironmentVariable("HOME");
            var launchAgentsPath = Path.Combine(homeDir, "Library", "LaunchAgents");
            
            if (!Directory.Exists(launchAgentsPath))
                Directory.CreateDirectory(launchAgentsPath);
            
            var plistPath = Path.Combine(launchAgentsPath, $"{appname}.startup.plist");
            
            var exePath = "/Applications/Rabbit.app/Contents/MacOS/YouYan.Rabbit";

            if (OperatingSystem.IsMacOS()) AutoStartHelper.MacOSSetAutoStart(plistPath, appname, exePath, value);
        }

        autoStartService.Set(value);
        messageBarService.ShowSuccess(value ? Language.SetAutomatically : Language.CancelAutomaticStart,
            Language.Message, true);
    }

    partial void OnSelectedLanguageChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
            return;

        if (languageService.Get() == value)
            return;

        languageService.Set(value);
        DialogServer.ShowMessageDialogAsync(Language.TakeEffectAfterRestart, Language.Message,
            Language.Ok);
    }

    private void InitLanguages()
    {
        Languages.Add(new KeyValuePair<string, string>("English", "en-US"));
        Languages.Add(new KeyValuePair<string, string>("中文", "zh-hans"));

        SelectedLanguage = languageService.Get();
    }
}