using TabbyCat.Repository.Entities.AiEntities;

namespace TabbyCat.IServices;

/// <summary>
/// 聊天会话
/// </summary>
public interface IChatSessionSyncService : IIncrementSyncService<IEnumerable<AiChatSessionEntity>>;