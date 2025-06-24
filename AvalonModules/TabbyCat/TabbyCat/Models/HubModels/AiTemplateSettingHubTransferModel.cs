using TabbyCat.Enums;
using TabbyCat.Repository.Entities.AiEntities;

namespace TabbyCat.Models.HubModels;

/// <summary>
/// Ai配置hub传送类
/// </summary>
/// <param name="data"></param>
/// <param name="transferType"></param>
public sealed class AiTemplateSettingHubTransferModel(
    IEnumerable<AiTemplateSettingEntity> data,
    HubTransferType transferType)
    : HubTransferModelBase<IEnumerable<AiTemplateSettingEntity>>(data, transferType)
{
}