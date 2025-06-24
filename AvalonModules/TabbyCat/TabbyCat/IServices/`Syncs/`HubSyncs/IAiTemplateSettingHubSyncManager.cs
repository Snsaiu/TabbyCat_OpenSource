using TabbyCat.Models.HubModels;
using TabbyCat.Repository.Entities.AiEntities;

namespace TabbyCat.IServices;

/// <summary>
/// ai配置信息增量同步管理器
/// </summary>
public interface IAiTemplateSettingHubSyncManager : IIncrementHubSyncManager<AiTemplateSettingEntity,
    AiTemplateSettingHubTransferModel, AiTemplateSettingEntity>
{
}