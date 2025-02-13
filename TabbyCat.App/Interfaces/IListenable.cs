namespace TabbyCat.App.Interfaces;

public interface IListenable
{
    Task ReceiveAsync(Action<object> receivedCallBack, CancellationToken cancellationToken);
}

public interface IListenable<T> : IListenable
{
    Task ReceiveAsync(Action<T> receivedCallBack, CancellationToken cancellationToken);

    Task IListenable.ReceiveAsync(Action<object> receivedCallBack, CancellationToken cancellationToken)
    {
        if (receivedCallBack is Action<T> action)
            receivedCallBack(action);
        else
            throw new NotImplementedException();
        return Task.CompletedTask;
    }


}