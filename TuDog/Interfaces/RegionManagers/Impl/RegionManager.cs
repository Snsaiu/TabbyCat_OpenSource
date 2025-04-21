using Avalonia.Controls;
using Avalonia.Interactivity;

using TuDog.Bases.Regions;
using TuDog.Bootstrap;
using TuDog.Extensions;
using TuDog.Interfaces.Navigations;
using TuDog.IocContainers;
using TuDog.ViewLocators;

namespace TuDog.Interfaces.RegionManagers.Impl;

public class RegionManager(RegionContainerBase regionContainer, IContainer container, ViewLocatorBase viewLocatorBase)
    : IRegionManager
{
    public void AddToRegion<T>(string regionName) where T : TuDogViewModelBase => BuildControlReturnVm<T>(regionName);

    private T BuildControlReturnVm<T>(string regionName) where T : TuDogViewModelBase
    {
        if (!regionContainer.Exists(regionName))
            throw new ArgumentException("Region not found");
        var vm = container.GetRequiredService<T>();
        BuildControl(regionName, vm);
        return vm;
    }

    private T BuildControlReturnVmAsync<T>(string regionName) where T : TuDogViewModelBase, IViewModelResult => BuildControlReturnVm<T>(regionName);
    private T BuildControlReturnVmAsync<T, TResult>(string regionName) where T : TuDogViewModelBase, IViewModelResult<TResult> => BuildControlReturnVm<T>(regionName);

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

    public void AddToRegion(string regionName, Type vmType) => BuildControlReturnVm(regionName, vmType);

    private object BuildControlReturnVm(string regionName, Type vmType)
    {
        if (!regionContainer.Exists(regionName))
            throw new ArgumentException("Region not found");
        var vm = container.GetRequiredService(vmType);
        BuildControl(regionName, vm);
        return vm;
    }

    public void AddToRegion<T>(string regionName, object? parameter) where T : ParameterViewModelBase => BuildControlReturnVm<T>(regionName, parameter);

    private T BuildControlReturnVm<T>(string regionName, object? parameter) where T : ParameterViewModelBase
    {
        if (!regionContainer.Exists(regionName))
            throw new ArgumentException("Region not found");
        var vm = container.GetRequiredService<T>();

        if (vm is ParameterViewModelBase parameterViewModelBase)
        {
            parameterViewModelBase.Parameter = parameter;
            BuildControl(regionName, parameterViewModelBase);
            return vm;
        }

        throw new ArgumentException("ParameterViewModelBase not found");
    }
    

    public IViewModelResult AddToRegionForResult<T>(string regionName) where T : TuDogViewModelBase, IViewModelResult => BuildControlReturnVm<T>(regionName);

    public IViewModelResult AddToRegionForResult<T>(string regionName, object? parameter) where T : ParameterViewModelBase, IViewModelResult => BuildControlReturnVm<T>(regionName, parameter);

    public IViewModelResultAsync<TResult> AddToRegionForResultAsync<T, TResult>(string regionName)
        where T : TuDogViewModelBase, IViewModelResultAsync<TResult> =>
        BuildControlReturnVmAsync<T, TResult>(regionName);

    public IViewModelResult<TResult> AddToRegionForResult<T, TResult>(string regionName)
        where T : TuDogViewModelBase, IViewModelResult<TResult> =>
        BuildControlReturnVmAsync<T, TResult>(regionName);


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