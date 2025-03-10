using CommunityToolkit.Mvvm.ComponentModel;

namespace TuDog.Bootstrap;

public abstract partial class ParameterViewModelBase : TuDogViewModelBase
{
    [ObservableProperty] private object? parameter;
}