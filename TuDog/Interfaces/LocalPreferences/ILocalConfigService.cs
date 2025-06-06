namespace TabbyCat.IServices.LocalConfigs;

public interface ILocalConfigService<T>
{
    string Key { get; }

    void SetNull();

    T Default { get; }
    T Get();
    T? GetOrDefault();
    void Set(T value);
}