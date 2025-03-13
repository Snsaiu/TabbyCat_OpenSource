using Microsoft.Extensions.DependencyInjection;
using TabbyCat.IServices.LocalConfigs;
using TuDog.Bootstrap;

namespace TuDog.Interfaces.PreferenceServices.Impl;

public abstract class LocalConfigService<T>() : ILocalConfigService<T>
{
    private IPreferenceService PreferenceService => TuDogApplication.ServiceProvider.GetService<IPreferenceService>();

    public abstract string Key { get; }

    public virtual T Default { get; } = default;

    public T Get()
    {
        return PreferenceService.Get(Key, Default);
    }

    public void Set(T value)
    {
        PreferenceService.Set(Key, value);
    }
}