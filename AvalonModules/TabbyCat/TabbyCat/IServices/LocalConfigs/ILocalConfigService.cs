namespace TabbyCat.IServices.LocalConfigs;

public interface ILocalConfigService<T>
{
    string Key { get; }

    T Default { get; }
    T Get();
    void Set(T value);
}