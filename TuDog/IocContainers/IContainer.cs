using Microsoft.Extensions.DependencyInjection;

namespace TuDog.IocContainers;

public interface IContainer
{
    public  T GetRequiredService<T>() where T : notnull;

    public object GetRequiredService(Type serviceType);

}