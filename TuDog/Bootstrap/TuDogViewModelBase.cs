using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TuDog.Bootstrap;

public abstract partial class TuDogViewModelBase:ModelBase
{
    [RelayCommand]
    private Task Loaded()
    {
        return OnLoaded();
    }

    protected virtual Task OnLoaded()
    {
        return Task.CompletedTask;
            
    }
    
    [RelayCommand]
    private Task UnLoaded()
    {
        return OnUnLoaded();
    }
    
    protected virtual Task OnUnLoaded()
    {
        return Task.CompletedTask;
    }
}