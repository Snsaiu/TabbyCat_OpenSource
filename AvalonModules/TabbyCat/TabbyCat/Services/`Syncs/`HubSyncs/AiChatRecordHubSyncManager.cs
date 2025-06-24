using Microsoft.Extensions.DependencyInjection;
using TabbyCat.IServices;
using TabbyCat.Models.HubModels;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;
using TuDog.IocAttribute;

namespace TabbyCat.Services;

[Register<IAiChatRecordHubSyncManager>(ServiceLifetime.Singleton)]
public sealed class AiChatRecordHubSyncManager(IAiChatMessageRecordService localService) :
    HubSyncManager<AiChatMessageRecordEntity, IAiChatMessageRecordService, Guid, AiChatRecordHubTransferModel>(
        localService),
    IAiChatRecordHubSyncManager
{
    public override string HubNotifyMethodName { get; } = "UpdateChatRecord";
    public override string SendHubMethodName { get; } = "UpdateChatRecordAsync";
    public override Func<IEnumerable<AiChatMessageRecordEntity>, Task>? UpdatedCallBack { get; set; }
}