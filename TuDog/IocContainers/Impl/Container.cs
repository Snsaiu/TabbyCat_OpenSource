using Microsoft.Extensions.DependencyInjection;

namespace TuDog.IocContainers.Impl;

public class Container(IServiceProvider provider) : IContainer
{
    public T GetRequiredService<T>() where T : notnull => provider.GetRequiredService<T>();
    public object GetRequiredService(Type serviceType) => provider.GetRequiredService(serviceType);
}