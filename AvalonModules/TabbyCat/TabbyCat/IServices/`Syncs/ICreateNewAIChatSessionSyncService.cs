namespace TabbyCat.IServices;

/// <summary>
/// 创建新的会话将本地数据发送到远程
/// </summary>
public interface ICreateNewAIChatSessionSyncService
{
    Task UpdateAsync();
}