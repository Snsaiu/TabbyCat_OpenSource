using TabbyCat.Shared.Interfaces;

namespace TabbyCat.App.Interfaces.Impls.HttpsTransfer;

public abstract class HttpsSendBase<T, P> : SendBase<T, P, HttpClient>
    where T : IFlag, ISize, ITargetFlag where P : IProgressValue
{
    public int SendPort { get; set; }

    public sealed override Task SendAsync(T message, IProgress<P>? progress, CancellationToken cancellationToken)
    {
        var client = new HttpClient();
        return SendProcessAsync(client, message, progress, cancellationToken);
    }
}