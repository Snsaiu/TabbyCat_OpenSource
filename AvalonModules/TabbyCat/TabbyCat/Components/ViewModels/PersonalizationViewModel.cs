using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels;
using TuDog.IocAttribute;

namespace TabbyCat.Components.ViewModels;

[Register]
public sealed partial class PersonalizationViewModel(ILanguageService languageService) : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<KeyValuePair<string, string>> languages = [];

    [ObservableProperty] private string selectedLanguage = string.Empty;

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

    private void InitLanguages()
    {
        Languages.Add(new("English", "en-US"));
        Languages.Add(new("中文", "zh-hans"));

        SelectedLanguage = languageService.Get();
    }
}