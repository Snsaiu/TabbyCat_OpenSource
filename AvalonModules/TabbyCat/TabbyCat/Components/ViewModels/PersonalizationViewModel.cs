using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using TabbyCat.Enums;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models.Users.Configs;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels;
using TuDog.IocAttribute;
using ViewModelBase = TabbyCat.ViewModels.Bases.ViewModelBase;

namespace TabbyCat.Components.ViewModels;

[Register]
public sealed partial class PersonalizationViewModel(
    ILanguageService languageService,
    IBackgroundImageConfig backgroundImageConfig,
    IBackgroundImageConfigService backgroundImageConfigService) : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<KeyValuePair<string, string>> _languages = [];

    [ObservableProperty] private string _selectedLanguage = string.Empty;

    [ObservableProperty] private BackgroundImageStatus _backgroundImageStatus = backgroundImageConfig.Status;

    [ObservableProperty] private string? _selectedCustomImage = backgroundImageConfig.CustomImage;

    [ObservableProperty] private double opacity = backgroundImageConfig.Opacity;

    protected override Task OnLoaded()
    {
        InitLanguages();
        return base.OnLoaded();
    }

    partial void OnSelectedLanguageChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
            return;

        if (languageService.Get() == value)
            return;

        languageService.Set(value);
        DialogServer.ShowMessageDialogAsync(AppResources.TakeEffectAfterRestart,AppResources.Message,AppResources.Ok);
    }

    partial void OnBackgroundImageStatusChanged(BackgroundImageStatus value)
    {
        backgroundImageConfig.Status = value;

        if (value is BackgroundImageStatus.Custom && !string.IsNullOrEmpty(backgroundImageConfig.CustomImage) &&
            File.Exists(backgroundImageConfig.CustomImage))
            SelectedCustomImage = backgroundImageConfig.CustomImage;

        backgroundImageConfigService.Set((BackgroundImageConfig)backgroundImageConfig);
    }

    partial void OnSelectedCustomImageChanged(string? value)
    {
        backgroundImageConfig.CustomImage = value;
        backgroundImageConfigService.Set((BackgroundImageConfig)backgroundImageConfig);
    }

    partial void OnOpacityChanged(double value)
    {
        backgroundImageConfig.Opacity = value;
        backgroundImageConfigService.Set((BackgroundImageConfig)backgroundImageConfig);
    }

    private void InitLanguages()
    {
        Languages.Add(new("English", "en-US"));
        Languages.Add(new("中文", "zh-hans"));

        SelectedLanguage = languageService.Get();
    }
}