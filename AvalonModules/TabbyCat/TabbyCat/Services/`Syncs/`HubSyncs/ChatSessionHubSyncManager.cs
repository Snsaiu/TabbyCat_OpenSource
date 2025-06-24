using Microsoft.Extensions.DependencyInjection;
using TabbyCat.IServices;
using TabbyCat.Models.HubModels;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;
using TuDog.IocAttribute;

namespace TabbyCat.Services;

[Register<IChatSessionHubSyncManager>(ServiceLifetime.Singleton)]
public sealed class ChatSessionHubSyncManager(IAiChatSessionService aiChatSessionService)
    : IncrementHubSyncManagerBase<AiChatSessionEntity, IAiChatSessionService, Guid,
        AiSessionHubTransferModel>(aiChatSessionService), IChatSessionHubSyncManager
{
    public override string HubNotifyMethodName { get; } = "UpdateChatSession";
    public override string SendHubMethodName { get; } = "UpdateChatSessionAsync";
    public override Func<IEnumerable<AiChatSessionEntity>, Task>? UpdatedCallBack { get; set; }
}