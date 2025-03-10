using TuDog.Interfaces;

namespace TuDog.Bootstrap;

public abstract class DialogViewModelBase<TResult> : DialogViewModelBase
{
    protected abstract Task<TResult?> OnConfirmAsync();

    protected virtual Task<TResult?> OnCancelAsync()
    {
        return Task.FromResult(default(TResult?));
    }

    public sealed override async Task<object?> ConfirmAsync()
    {
        var result = await OnConfirmAsync();
        return result;
    }

    public sealed override async Task<object?> CancelAsync()
    {
        var result = await OnCancelAsync();
        return result;
    }
}

public abstract class DialogViewModelBase : ParameterViewModelBase, IViewModelResultAsync
{
    public virtual Task<bool> CanCancelAsync()
    {
        return Task.FromResult(true);
    }

    public virtual Task<bool> CanConfirmAsync()
    {
        return Task.FromResult(true);
    }


    public abstract Task<object?> ConfirmAsync();
    public abstract Task<object?> CancelAsync();
}