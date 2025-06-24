using TabbyCat.Models.HubModels;
using TabbyCat.Repository.Entities.AiEntities;

namespace TabbyCat.IServices;

/// <summary>
/// Ai聊天会话同步管理器
/// </summary>
public interface
    IChatSessionHubSyncManager : IIncrementHubSyncManager<AiChatSessionEntity, AiSessionHubTransferModel,
    AiChatSessionEntity>;