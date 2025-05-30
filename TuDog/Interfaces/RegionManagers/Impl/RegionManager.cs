using Avalonia.Controls;
using Avalonia.Interactivity;
using TuDog.Bases.Regions;
using TuDog.Bootstrap;
using TuDog.Extensions;
using TuDog.IocContainers;
using TuDog.ViewLocators;

namespace TuDog.Interfaces.RegionManagers.Impl;

public class RegionManager(RegionContainerBase regionContainer, IContainer container, ViewLocatorBase viewLocatorBase)
    : IRegionManager
{
    public void AddToRegion<T>(string regionName) where T : TuDogViewModelBase
    {
        BuildControlReturnVm<T>(regionName);
    }

    private Dictionary<Type, object> _keepVmDictionary = [];


    private T? KeepParse<T>()
    {
        if (!_keepVmDictionary.ContainsKey(typeof(T))) return default;

        var value = _keepVmDictionary[typeof(T)];
        switch (value)
        {
            case IKeep { Keep: true }:
                return (T)value;
            case IKeep { Keep: false }:
                _keepVmDictionary.Remove(typeof(T));
                break;
        }

        return default;
    }

    private T BuildControlReturnVm<T>(string regionName) where T : TuDogViewModelBase
    {
        if (!regionContainer.Exists(regionName))
            throw new ArgumentException("Region not found");

        var vm = KeepParse<T>() ?? container.GetRequiredService<T>();

        BuildControl(regionName, vm);

        if (vm is IKeep { Keep: true } && !_keepVmDictionary.ContainsKey(typeof(T)))
            _keepVmDictionary.Add(typeof(T), vm);

        return vm;
    }

    private T BuildControlReturnVmAsync<T>(string regionName) where T : TuDogViewModelBase, IViewModelResult
    {
        return BuildControlReturnVm<T>(regionName);
    }

    private T BuildControlReturnVmAsync<T, TResult>(string regionName)
        where T : TuDogViewModelBase, IViewModelResultAsync<TResult>
    {
        return BuildControlReturnVm<T>(regionName);
    }

    // private T BuildControlReturnVmAsync<T, TResult>(string regionName) where T : TuDogViewModelBase, IViewModelResultAsync<TResult> => BuildControlReturnVm<T>(regionName);

    private void BuildControl(string regionName, object vm)
    {
        if (vm is not (TuDogViewModelBase and var tuDogViewModelBase))
            throw new ArgumentException("VM is not of type TuDogViewModelBase");

        var control = viewLocatorBase.Build(vm);
        if (control is null)
            return;
        control.DataContext = vm;
        regionContainer.GetRegion(regionName).Content = control;
        control.AttachLoadedBehavior(tuDogViewModelBase);
        control.AttachUnLoadedBehavior(tuDogViewModelBase);
        control.Unloaded += UnRemoveRegion;
    }

    public void AddToRegion(string regionName, Type vmType)
    {
        BuildControlReturnVm(regionName, vmType);
    }

    public T? GetKeepViewModel<T>() where T : TuDogViewModelBase
    {
        if (_keepVmDictionary.TryGetValue(typeof(T), out var vm))
            return (T)vm;

        return null;
    }

    public TuDogViewModelBase AddToRegionReturnViewModel(string regionName, Type vmType)
    {
        return BuildControlReturnVm(regionName, vmType) as TuDogViewModelBase ?? throw new NullReferenceException();
    }

    private object BuildControlReturnVm(string regionName, Type vmType)
    {
        if (!regionContainer.Exists(regionName))
            throw new ArgumentException("Region not found");

        object? vm = null;
        if (_keepVmDictionary.ContainsKey(vmType))
        {
            vm = _keepVmDictionary[vmType];
            if (vm is IKeep { Keep: false }) _keepVmDictionary.Remove(vmType);
        }

        vm ??= container.GetRequiredService(vmType);

        BuildControl(regionName, vm);
        if (vm is IKeep { Keep: true } && !_keepVmDictionary.ContainsKey(vmType)) _keepVmDictionary.Add(vmType, vm);

        return vm;
    }

    public void AddToRegion<T>(string regionName, object? parameter) where T : IParameter
    {
        BuildControlReturnVm<T>(regionName, parameter);
    }

    private T BuildControlReturnVm<T>(string regionName, object? parameter) where T : IParameter
    {
        if (!regionContainer.Exists(regionName))
            throw new ArgumentException("Region not found");

        var vm = KeepParse<T>();
        vm ??= container.GetRequiredService<T>();

        if (vm is IParameter parameterViewModelBase)
        {
            parameterViewModelBase.Parameter = parameter;
            BuildControl(regionName, parameterViewModelBase);

            if (vm is IKeep { Keep: true }) _keepVmDictionary.Add(typeof(T), vm);

            return vm;
        }

        throw new ArgumentException("ParameterViewModelBase not found");
    }


    public IViewModelResult AddToRegionForResult<T>(string regionName) where T : TuDogViewModelBase, IViewModelResult
    {
        return BuildControlReturnVm<T>(regionName);
    }

    public IViewModelResult AddToRegionForResult<T>(string regionName, object? parameter)
        where T : IParameter, IViewModelResult
    {
        return BuildControlReturnVm<T>(regionName, parameter);
    }

    public IViewModelResultAsync<TResult> AddToRegionForResultAsync<T, TResult>(string regionName)
        where T : TuDogViewModelBase, IViewModelResultAsync<TResult>
    {
        return BuildControlReturnVmAsync<T, TResult>(regionName);
    }

    // public IViewModelResult<TResult> AddToRegionForResult<T, TResult>(string regionName)
    //     where T : TuDogViewModelBase, IViewModelResult<TResult> =>
    //     BuildControlReturnVmAsync<T, TResult>(regionName);
    //

    private void UnRemoveRegion(object? sender, RoutedEventArgs e)
    {
        //todo: remove region 如果使用递归，那么可能会有误删的情况
        // if(sender is not Control control)return;
        // foreach (var item in control.GetLogicalChildren().OfType<Control>())
        // {
        //     var regionName = RegionBehavior.GetRegion(control);
        //     if (!string.IsNullOrEmpty(regionName))
        //     {
        //         _regionContainer.Remove(regionName);
        //     }
        // }
    }

    public Control GetViewByViewModel<T>() where T : TuDogViewModelBase
    {
        var vm = container.GetRequiredService<T>();
        var control = viewLocatorBase.Build(vm);
        if (control is null)
            throw new NullReferenceException();

        control.DataContext = vm;
        control.AttachLoadedBehavior(vm);
        control.AttachUnLoadedBehavior(vm);
        control.Unloaded += UnRemoveRegion;
        return control;
    }
}