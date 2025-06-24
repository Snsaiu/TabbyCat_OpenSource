using TabbyCat.Enums;
using TabbyCat.Repository.Entities.AiEntities;

namespace TabbyCat.Models.HubModels;

/// <summary>
/// AI聊天记录hub传送类
/// </summary>
/// <param name="data"></param>
/// <param name="transferType"></param>
public sealed class AiChatRecordHubTransferModel(
    IEnumerable<AiChatMessageRecordEntity> data,
    HubTransferType transferType)
    : HubTransferModelBase<IEnumerable<AiChatMessageRecordEntity>>(data, transferType)
{
}