namespace TuDog.Interfaces.PreferenceServices;

public interface IPreferenceService
{
    public void Set<T>(string key, T value);

    public T Get<T>(string key, T defaultValue);
}