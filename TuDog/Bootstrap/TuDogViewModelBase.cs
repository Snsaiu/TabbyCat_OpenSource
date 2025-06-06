using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TuDog.Bootstrap;


public interface ITuDogViewModel
{
    IAsyncRelayCommand LoadedCommand { get; }
    IAsyncRelayCommand UnLoadedCommand { get; }
}

public abstract class TuDogViewModelBase:ModelBase, ITuDogViewModel
{
    
    protected virtual Task OnLoaded()
    {
        return Task.CompletedTask;
            
    }
    
    
    protected virtual Task OnUnLoaded()
    {
        return Task.CompletedTask;
    }

    public TuDogViewModelBase()
    {
        this.LoadedCommand=new AsyncRelayCommand(OnLoaded);
        this.UnLoadedCommand=new AsyncRelayCommand(OnUnLoaded);
    }

    public IAsyncRelayCommand LoadedCommand { get; }
    public IAsyncRelayCommand UnLoadedCommand { get; }
}