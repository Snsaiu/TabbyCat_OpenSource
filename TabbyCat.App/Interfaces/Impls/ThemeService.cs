using Microsoft.FluentUI.AspNetCore.Components;

namespace TabbyCat.App.Interfaces.Impls;

public class ThemeService : IThemeService
{
    public DesignThemeModes GetDesignTheme()
    {
        return (DesignThemeModes)Preferences.Default.Get<int>("theme", (int)DesignThemeModes.System);
    }

    public OfficeColor GetThemeColor()
    {
        return (OfficeColor)Preferences.Default.Get<int>("color", (int)OfficeColor.Default);
    }

    public void SetThemeColor(OfficeColor color)
    {
        Preferences.Default.Set<int>("color", (int)color);
    }

    public void SetDesignTheme(DesignThemeModes theme)
    {
        Preferences.Default.Set<int>("theme", (int)theme);
    }
}