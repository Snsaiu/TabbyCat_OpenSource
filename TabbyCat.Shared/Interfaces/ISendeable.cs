namespace TabbyCat.Shared.Interfaces;


/// <summary>
/// 可发送数据
/// </summary>
public interface ISendeable
{
    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="message">数据</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendAsync(object message, CancellationToken cancellationToken);
}

public interface ISendeable<in TMessage> : ISendeable
{
    Task SendAsync(TMessage message, CancellationToken cancellationToken);

    Task ISendeable.SendAsync(object message, CancellationToken cancellationToken)
    {
        return SendAsync(message, cancellationToken);
    }
}