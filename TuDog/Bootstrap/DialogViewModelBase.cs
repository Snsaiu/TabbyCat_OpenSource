using TuDog.Interfaces;

namespace TuDog.Bootstrap;

public abstract class DialogViewModelBase:TuDogViewModelBase, IViewModelResult,IParameter
{

    public abstract object Confirm();

    public abstract object Cancel();
    
    public virtual bool CanConfirm()
    {
        return true;
    }

    public object? Parameter { get; set; }
}

public abstract class DialogViewModelBaseAsync : TuDogViewModelBase, IViewModelResultAsync, IParameter
{
    public abstract Task<object?> ConfirmAsync();


    public abstract Task<object?> CancelAsync();
 

    public Task<bool> CanConfirmAsync()
    {
        return Task.FromResult(true);
    }

    public object? Parameter { get; set; }
}

public abstract class DialogViewModelBase<TParameter,TResult> : TuDogViewModelBase, IViewModelResult<TResult>,IParameter<TParameter>
{
    private TParameter? _parameter;
    public abstract TResult Confirm();


    public abstract TResult Cancel();
  
    public bool CanConfirm()
    {
        return true;
    }
    
    public TParameter? Parameter { get; set; }
}

public abstract class DialogViewModelBaseAsync<TParameter,TResult> : TuDogViewModelBase, IViewModelResultAsync<TResult>,IParameter<TParameter>
{
    public Task<bool> CanConfirmAsync()
    {
        return Task.FromResult(true);
    }

    public abstract Task<TResult> ConfirmAsync();


    public abstract Task<TResult> CancelAsync();

    public TParameter? Parameter { get; set; }
}

