using TabbyCat.Shared.Interfaces;

namespace AirTransfer.Interfaces.Impls;

public abstract class SendBase<T, P, Sender> : ISendableWithProgress<T, P>
    where T : IFlag, ISize, ITargetFlag where P : IProgressValue
{
    public abstract Task SendAsync(T message, IProgress<P>? progress, CancellationToken cancellationToken);


    protected abstract Task SendProcessAsync(Sender sender, T message, IProgress<P>? progress,
        CancellationToken cancellationToken);
}