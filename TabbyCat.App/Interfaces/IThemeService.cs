using Microsoft.FluentUI.AspNetCore.Components;

namespace TabbyCat.App.Interfaces;

public interface IThemeService
{
    DesignThemeModes GetDesignTheme();

    OfficeColor GetThemeColor();

    void SetThemeColor(OfficeColor color);

    void SetDesignTheme(DesignThemeModes theme);
}