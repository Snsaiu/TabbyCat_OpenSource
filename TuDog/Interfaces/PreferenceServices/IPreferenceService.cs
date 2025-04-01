namespace TuDog.Interfaces.PreferenceServices;

public interface IPreferenceService
{
    void Set<T>(string key, T value);

    void SetNull(string key);

    T? GetOrDefault<T>(string key);

    T Get<T>(string key, T defaultValue);
}