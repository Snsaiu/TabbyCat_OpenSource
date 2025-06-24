using TabbyCat.Models.HubModels;
using TabbyCat.Repository.Entities.AiEntities;

namespace TabbyCat.IServices;

/// <summary>
/// Ai聊天记录同步管理器
/// </summary>
public interface IAiChatRecordHubSyncManager : IHubSyncManager<AiChatMessageRecordEntity, AiChatRecordHubTransferModel,
    AiChatMessageRecordEntity>;