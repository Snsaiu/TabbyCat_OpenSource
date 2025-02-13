namespace TabbyCat.App.Interfaces;

/// <summary>
/// 检查指定的端口是否被占用
/// </summary>
public interface IPortCheckable
{
    /// <summary>
    /// 对输入的端口进行检查是否已经被占用
    /// </summary>
    /// <param name="port">要检查的端口</param>
    /// <returns>已经被占用返回true，否则返回false</returns>
    Task<bool> IsPortInUse(int port);
}