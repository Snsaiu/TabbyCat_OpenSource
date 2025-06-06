using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using TuDog.Bootstrap;
using TuDog.Interfaces.RegionManagers;

namespace TuDog.Interfaces.Navigations.Impl;

public sealed class NavigationService(IApplicationLifetime singleViewPlatform) : INavigationService
{
    private Stack<Control?> stack = new();

    private IRegionManager _regionManager = TuDogApplication.ServiceProvider.GetRequiredService<IRegionManager>();

    public Task PushAsync<ViewModel>(INavigationParameter? parameter) where ViewModel : TuDogViewModelBase
    {
        
        var control = _regionManager.GetViewByViewModel<ViewModel>();
        
        if (singleViewPlatform is IClassicDesktopStyleApplicationLifetime desktop)
        {
            throw new NotImplementedException();
        }
        else if (singleViewPlatform is ISingleViewApplicationLifetime view)
        {
            stack.Push(view.MainView);
            view.MainView = control;
            if (control.DataContext is INavigationViewModel viewModel)
            {
                viewModel.OnPushHereAsync(parameter);
            }
            TuDogApplication.TopLevel = TopLevel.GetTopLevel(control);
        }

        return Task.CompletedTask;
    }

    public Task PopAsync(INavigationParameter? result=null)
    {
        if(stack.Count == 0)
            return Task.CompletedTask;

        var control = stack.Pop();
        
        if(control is null)
            throw new InvalidOperationException();

        if (singleViewPlatform is IClassicDesktopStyleApplicationLifetime desktop)
        {
            throw new NotImplementedException();
        }
        else if (singleViewPlatform is ISingleViewApplicationLifetime view)
        {
            view.MainView = control;
            if (control.DataContext is INavigationViewModel viewModel)
            {
                viewModel.OnPopHereAsync(result);
            }
            TuDogApplication.TopLevel = TopLevel.GetTopLevel(control);
        }
        
        return Task.CompletedTask;
    }
}