namespace TuDog.Interfaces;


public interface IViewModelResult
{
    object Confirm();
    object Cancel();
}

public interface IViewModelResultAsync
{
    Task<object?> ConfirmAsync();
    Task<object?> CancelAsync();
}

public interface IViewModelResult<out TResult> : IViewModelResult
{
    new TResult Confirm();
    new TResult Cancel();

    object IViewModelResult.Confirm()
    {
        return Confirm();
    }

    object IViewModelResult.Cancel()
    {
        return Cancel();
    }
}

public interface IViewModelResultAsync<TResult> : IViewModelResult<TResult>
{
    Task<TResult> ConfirmAsync();
    Task<TResult> CancelAsync();

    TResult IViewModelResult<TResult>.Confirm()
    {
        return ConfirmAsync().GetAwaiter().GetResult();
    }

    TResult IViewModelResult<TResult>.Cancel()
    {
        return CancelAsync().GetAwaiter().GetResult();
    }

    object IViewModelResult.Confirm()
    {
        return ConfirmAsync().GetAwaiter().GetResult();
    }

    object IViewModelResult.Cancel()
    {
        return CancelAsync().GetAwaiter().GetResult();
    }
}

public interface IDialogViewModelResult
{
}