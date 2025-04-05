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
        if( Confirm() is not null and var v)
            return v;
        throw new NullReferenceException("Confirm() is null");
    }

    object IViewModelResult.Cancel()
    {
        if( Cancel() is not null and var v)
            return v;
        throw new NullReferenceException("Cancel() is null");
    }
}

public interface IViewModelResultAsync<TResult> : IViewModelResult<TResult>
{
    Task<TResult> ConfirmAsync();
    Task<TResult> CancelAsync();

    TResult IViewModelResult<TResult>.Confirm()
    {
        if( ConfirmAsync().GetAwaiter().GetResult() is not null and var v)
            return v;
        throw new NullReferenceException("ConfirmAsync() is null");
    }

    TResult IViewModelResult<TResult>.Cancel()
    {
        return CancelAsync().GetAwaiter().GetResult();
    }

    object IViewModelResult.Confirm()
    {
        if( ConfirmAsync().GetAwaiter().GetResult() is not null and var v)
            return v;
        throw new NullReferenceException("ConfirmAsync() is null");
    }

    object IViewModelResult.Cancel()
    {
        if( CancelAsync().GetAwaiter().GetResult() is not null and var v)
            return v;
        throw new NullReferenceException("CancelAsync() is null");
    }
}

public interface IDialogViewModelResult
{
}