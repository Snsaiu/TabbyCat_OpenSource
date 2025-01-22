namespace TabbyCat.Shared.Interfaces;

/// <summary>
/// 发送数据并且带有回调进度
/// </summary>
public interface ISendableWithProgress : ISendeable
{
    Task SendAsync(object message, IProgress<object>? progress, CancellationToken cancellationToken);

    Task ISendeable.SendAsync(object message, CancellationToken cancellationToken)
    {
        return SendAsync(message, null, cancellationToken);
    }
}

public interface ISendableWithProgress<in T, out P> : ISendableWithProgress where P : IProgressValue
{
    Task SendAsync(T message, IProgress<P>? progress, CancellationToken cancellationToken);

    Task ISendableWithProgress.SendAsync(object message, IProgress<object>? progress, CancellationToken cancellationToken)
    {
        return SendAsync(message, progress, cancellationToken);
    }
}