using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.Enums;
using TabbyCat.Extensions;
using TabbyCat.IServices;
using TabbyCat.Models.HubModels;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;
using TabbyCat.Shared.Languages;
using TuDog.IocAttribute;
using YouYan.Hamster.ResultModels;

namespace TabbyCat.Services;

[Register<IChatSessionSyncService>(ServiceLifetime.Singleton)]
public sealed class ChatSessionSyncService(
    IChatSessionHubSyncManager chatSessionSyncManager,
    IAiChatSessionService localService)
    : IncrementSyncServiceBase<IEnumerable<AiChatSessionEntity>, IAiChatSessionService, AiChatSessionEntity, Guid>(
            localService),
        IChatSessionSyncService
{
    protected override string QueryLatestVersionUrl =>
        $"/api/app/tabby-cat-chat-session/query-ai-chat-session-version-setting?email={User.Email}";

    protected override string SyncUrl => "/api/app/tabby-cat-chat-session/download-ai-chat-session";
    protected override string UploadUrl => "/api/app/tabby-cat-chat-session/upload-ai-chat-session";

    protected override async Task<IResultModel<bool>> UpdateLocalDbAsync(IEnumerable<AiChatSessionEntity> data)
    {
        foreach (var item in data)
            item.Email = User.Email;
        var result = await localService.AddRangeAsync(data);
        if (result)
        {
            return ResultModelFactory.Success<bool>(true);
        }
        else
        {
            Logger.LogError("Ai聊天会话配置远程数据更新到本地失败！");
            return ResultModelFactory.Error<bool>(AppResources.AnErrorOccurred);
        }
    }

    protected override Task UploadNewVersionToRemoteAsync(IEnumerable<AiChatSessionEntity> data)
    {
        return UploadRemoteAsync(data);
    }

    protected override Task OnUpdatedAsync(IEnumerable<AiChatSessionEntity> data)
    {
        return chatSessionSyncManager.SyncAsync(new AiSessionHubTransferModel(data, HubTransferType.Update));
    }
}