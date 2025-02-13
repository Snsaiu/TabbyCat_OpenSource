using System.Net;

using TabbyCat.Shared.Interfaces;

namespace TabbyCat.App.Interfaces;

public interface IReceiveableWithProgress : IListenable
{
    Task ReceiveAsync(Action<object> receivedCallBack, IProgress<object>? progress, CancellationToken cancellationToken);

    Task IListenable.ReceiveAsync(Action<object> receivedCallBack, CancellationToken cancellationToken)
    {
        return ReceiveAsync(receivedCallBack, null, cancellationToken);
    }
}

public interface IReceiveableWithProgress<out T, out P> : IReceiveableWithProgress where P : IProgressValue
{
    Task ReceiveAsync(Action<T> receivedCallBack, IPAddress address, int port, IProgress<P>? progress, CancellationToken cancellationToken);

    Task IReceiveableWithProgress.ReceiveAsync(Action<object> receivedCallBack, IProgress<object>? progress, CancellationToken cancellationToken)
    {
        return ReceiveAsync(receivedCallBack, progress, cancellationToken);
    }
}