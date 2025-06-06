using System.ComponentModel;
using System.Globalization;

namespace YouYan.Rabbit.Languages;


public class LocalizationResourceManager : INotifyPropertyChanged
{
    private LocalizationResourceManager()
    {
        Language.Culture = new("en-US");
    }

    private static LocalizationResourceManager _instance;
    
    
    public static LocalizationResourceManager Instance => _instance ??= new LocalizationResourceManager();

    public string this[string resourceKey] =>
        Language.ResourceManager.GetString(resourceKey, Language.Culture) ?? string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public void SetCulture(CultureInfo culture)
    {
        Language.Culture = culture;
        PropertyChanged?.Invoke(this, new(null));
    }
}