using Microsoft.Extensions.DependencyInjection;
using TabbyCat.IServices;
using TabbyCat.Models.HubModels;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;
using TuDog.IocAttribute;

namespace TabbyCat.Services;

[Register<ICustomOccupationHubSyncManager>(ServiceLifetime.Singleton)]
public sealed class CustomOccupationHubSyncManager(
    ICustomAssistantOccupationService customAssistantOccupationService)
    : IncrementHubSyncManagerBase<CustomAssistantOccupationEntity, ICustomAssistantOccupationService, Guid,
        CustomOccupationSettingHubTransferModel>(
        customAssistantOccupationService), ICustomOccupationHubSyncManager
{
    public override string HubNotifyMethodName { get; } = "UpdateCustomOccupation";
    public override string SendHubMethodName { get; } = "UpdateCustomOccupationAsync";
    public override Func<IEnumerable<CustomAssistantOccupationEntity>, Task>? UpdatedCallBack { get; set; }
}