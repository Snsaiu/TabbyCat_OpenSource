namespace TabbyCat.App.Interfaces;

public interface ILanguageService
{
    string? GetLanguage();

    void SetLanguage(string language);
}