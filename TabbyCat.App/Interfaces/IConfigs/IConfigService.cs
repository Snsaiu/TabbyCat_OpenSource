namespace TabbyCat.App.Interfaces.IConfigs;

public interface IConfigService
{
    string Key { get; }
    T Get<T>();
    void Set<T>(T value);
}