using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Models.HubModels;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;
using TuDog.IocAttribute;

namespace TabbyCat.Services;

[Register<IAiTemplateSettingHubSyncManager>(ServiceLifetime.Singleton)]
public sealed class AiTemplateSettingHubSyncManager(
    IAiTemplateSettingService aiTemplateSettingService) :
    IncrementHubSyncManagerBase<AiTemplateSettingEntity,
        IAiTemplateSettingService, Guid, AiTemplateSettingHubTransferModel>(aiTemplateSettingService),
    IAiTemplateSettingHubSyncManager
{
    public override string HubNotifyMethodName { get; } = "UpdateAiTemplateSetting";
    public override string SendHubMethodName { get; } = "UpdateAiTemplateSettingAsync";

    public override Func<IEnumerable<AiTemplateSettingEntity>, Task>? UpdatedCallBack { get; set; }
}