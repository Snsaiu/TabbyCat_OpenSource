using TabbyCat.Enums;
using TabbyCat.Repository.Entities.AiEntities;

namespace TabbyCat.Models.HubModels;

/// <summary>
/// 自定义联系人hub传送类
/// </summary>
/// <param name="data"></param>
/// <param name="transferType"></param>
public sealed class CustomOccupationSettingHubTransferModel(
    IEnumerable<CustomAssistantOccupationEntity> data,
    HubTransferType transferType)
    : HubTransferModelBase<IEnumerable<CustomAssistantOccupationEntity>>(data, transferType);