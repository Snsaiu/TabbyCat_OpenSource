using System.ComponentModel;
using System.Globalization;
using TabbyCat.Shared.Languages;

namespace TabbyCat.Shared;

public class LocalizationResourceManager : INotifyPropertyChanged
{
    private LocalizationResourceManager()
    {
        AppResources.Culture = new("en-US");
    }

    private static LocalizationResourceManager _instance;
    
    
    public static LocalizationResourceManager Instance => _instance ??= new LocalizationResourceManager();

    public string this[string resourceKey] =>
        AppResources.ResourceManager.GetString(resourceKey, AppResources.Culture) ?? string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public void SetCulture(CultureInfo culture)
    {
        AppResources.Culture = culture;
        PropertyChanged?.Invoke(this, new(null));
    }

    public CultureInfo GetCulture()
    {
        return AppResources.Culture;
    }
}