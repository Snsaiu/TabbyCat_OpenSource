namespace TabbyCat.App.Interfaces.Impls;

public class ConfigLanguageService : ILanguageService
{
    public void SetLanguage(string language)
    {
        Preferences.Default.Set<string>("language", language);
    }

    public string? GetLanguage()
    {
        return Preferences.Default.Get<string?>("language", string.Empty);
    }
}