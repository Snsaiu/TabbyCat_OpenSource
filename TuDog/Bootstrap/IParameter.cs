using CommunityToolkit.Mvvm.ComponentModel;

namespace TuDog.Bootstrap;

public interface IParameter
{
    object? Parameter { get; set; }
}

public interface IParameter<T> : IParameter
{
    T? Parameter { get; set; }
    
    object? IParameter.Parameter { get=>Parameter; set=>Parameter=(T?)value; }
}