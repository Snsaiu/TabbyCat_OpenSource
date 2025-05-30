using Avalonia.Controls;
using TuDog.Bootstrap;
using TuDog.Interfaces.Navigations;

namespace TuDog.Interfaces.RegionManagers;

public interface IRegionManager
{
    void AddToRegion<T>(string regionName) where T : TuDogViewModelBase;

    void AddToRegion(string regionName, Type vmType);

    T? GetKeepViewModel<T>() where T : TuDogViewModelBase;

    TuDogViewModelBase AddToRegionReturnViewModel(string regionName, Type vmType);

    void AddToRegion<T>(string regionName, object? parameter) where T : IParameter;

    Control GetViewByViewModel<T>() where T : TuDogViewModelBase;

    // IViewModelResult AddToRegionForResult<T>(string regionName) where T : TuDogViewModelBase, IViewModelResult;

    IViewModelResult AddToRegionForResult<T>(string regionName, object? parameter)
        where T : IParameter, IViewModelResult;

    // IViewModelResultAsync<TResult> AddToRegionForResultAsync<T, TResult>(string regionName) where T : TuDogViewModelBase, IViewModelResultAsync<TResult>;
    //
    // IViewModelResult<TResult> AddToRegionForResult<T, TResult>(string regionName) where T : TuDogViewModelBase, IViewModelResult<TResult>;
}