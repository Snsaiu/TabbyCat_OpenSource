using System;
using Microsoft.Extensions.DependencyInjection;

namespace TuDog.IocAttribute;



[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class RegisterAttribute : Attribute
{
    public ServiceLifetime Lifetime { get; }

    public RegisterAttribute(ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        Lifetime = lifetime;
    }
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class RegisterAttribute<TService> : Attribute
{
    public ServiceLifetime Lifetime { get; }

    public RegisterAttribute(ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        Lifetime = lifetime;
    }
}