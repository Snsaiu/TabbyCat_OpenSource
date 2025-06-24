using TabbyCat.Enums;
using TabbyCat.Repository.Entities.AiEntities;

namespace TabbyCat.Models.HubModels;

/// <summary>
/// AI聊天会话hub传送类
/// </summary>
/// <param name="data"></param>
/// <param name="transferType"></param>
public sealed class AiSessionHubTransferModel(IEnumerable<AiChatSessionEntity> data, HubTransferType transferType)
    : HubTransferModelBase<IEnumerable<AiChatSessionEntity>>(data, transferType);