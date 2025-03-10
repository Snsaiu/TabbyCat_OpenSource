using Avalonia.Controls;

using TuDog.Bootstrap;

namespace TuDog.Interfaces.RegionManagers;

public interface IRegionManager
{
    void AddToRegion<T>(string regionName) where T : TuDogViewModelBase;

    void AddToRegion(string regionName, Type vmType);

    void AddToRegion<T>(string regionName, object? parameter) where T : ParameterViewModelBase;

    Control GetViewByViewModel<T>() where T : TuDogViewModelBase;

    IViewModelResult AddToRegionForResult<T>(string regionName) where T : TuDogViewModelBase, IViewModelResult;

    IViewModelResult AddToRegionForResult<T>(string regionName, object? parameter) where T : ParameterViewModelBase, IViewModelResult;

    IViewModelResultAsync<TResult> AddToRegionForResultAsync<T, TResult>(string regionName) where T : TuDogViewModelBase, IViewModelResultAsync<TResult>;

    IViewModelResult<TResult> AddToRegionForResult<T, TResult>(string regionName) where T : TuDogViewModelBase, IViewModelResult<TResult>;
}