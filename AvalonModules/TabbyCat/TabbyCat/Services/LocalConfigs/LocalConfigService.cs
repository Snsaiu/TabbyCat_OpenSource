using TabbyCat.IServices.LocalConfigs;
using TuDog.Interfaces.PreferenceServices;

namespace TabbyCat.Services.LocalConfigs;

public abstract class LocalConfigService<T>(IPreferenceService preferenceService) : ILocalConfigService<T>
{
    public abstract string Key { get; }

    public virtual T Default { get; } = default;

    public T Get()
    {
        return preferenceService.Get(Key, Default);
    }

    public void Set(T value)
    {
        preferenceService.Set(Key, value);
    }
}