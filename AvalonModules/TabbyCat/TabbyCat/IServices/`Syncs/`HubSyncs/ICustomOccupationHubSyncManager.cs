using TabbyCat.Models.HubModels;
using TabbyCat.Repository.Entities.AiEntities;

namespace TabbyCat.IServices;

/// <summary>
/// 自定义角色同步管理器
/// </summary>
public interface ICustomOccupationHubSyncManager : IIncrementHubSyncManager<CustomAssistantOccupationEntity,
    CustomOccupationSettingHubTransferModel, CustomAssistantOccupationEntity>
{
}