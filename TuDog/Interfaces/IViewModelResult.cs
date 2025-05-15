using TuDog.Bootstrap;
using TuDog.Enums;

namespace TuDog.Interfaces;


public interface IViewModelResult:ITuDogViewModel
{
    object Confirm();
    object Cancel();

    bool CanConfirm();
}

public interface IViewModelResultAsync:ITuDogViewModel
{
    Action<string, string, MessageState> ErrorMessageAction { get; set; }
    Task<object?> ConfirmAsync();
    Task<object?> CancelAsync();

    Task<bool> CanConfirmAsync();
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

public interface IViewModelResultAsync<TResult> : IViewModelResultAsync
{
    Task<TResult> ConfirmAsync();
    Task<TResult> CancelAsync();

    Task<bool> CanConfirmAsync();


    async Task< object?> IViewModelResultAsync.ConfirmAsync()
    {
       return await ConfirmAsync();
    }

   async Task<object?> IViewModelResultAsync.CancelAsync()
   {
       var result = await CancelAsync();
       if (result is null)
           return null;
       return result;
   }

}