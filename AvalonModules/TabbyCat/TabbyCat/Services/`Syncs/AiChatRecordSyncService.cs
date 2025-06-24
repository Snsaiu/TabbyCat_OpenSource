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

/// <summary>
/// AI聊天记录同步服务
/// </summary>
[Register<IAiChatRecordSyncService>(ServiceLifetime.Singleton)]
public class AiChatRecordSyncService(
    IAiChatMessageRecordService chatMessageRecordService,
    IAiChatRecordHubSyncManager aiChatRecordHubSyncManager)
    : SyncServiceBase<IEnumerable<AiChatMessageRecordEntity>, DateTime>, IAiChatRecordSyncService
{
    protected override string UploadUrl { get; } = "/api/app/tabby-cat-chat-record/upload-chat";
    protected override string SyncUrl { get; } = "/api/app/tabby-cat-chat-record/sync";

    protected override async Task<IResultModel<bool>> WriteToLocalDbAsync(IEnumerable<AiChatMessageRecordEntity> data)
    {
        foreach (var item in data) item.SyncStatus = true;

        var result = await chatMessageRecordService.AddRangeAsync(data);
        if (result)
        {
            return ResultModelFactory.Success<bool>();
        }
        else
        {
            Logger.LogError("聊天记录从远程同步到本地失败");
            return ResultModelFactory.Error<bool>(AppResources.AnErrorOccurred);
        }
    }

    public override async Task<IResultModel<bool>> SyncAsync()
    {
        var time = new DateTime(0001, 01, 01);

        var chats = (await chatMessageRecordService.QueryAsync(x =>
            !x.SyncStatus && x.Email == User.Email && x.ChatRecordTime > time)).ToList();

        if (chats.Any())
            foreach (var item in chats)
            {
                await UploadRemoteAsync([item]);
                item.SyncStatus = true;
            }

        await chatMessageRecordService.UpdateRangeAsync(chats);

        return await base.SyncAsync();
    }

    protected override async Task<IResultModel<DateTime>> GetConditionAsync()
    {
        var result = await chatMessageRecordService.QueryAsync(x => x.Email == User.Email);
        var latestTime = result.MaxBy(x => x.ChatRecordTime)?.ChatRecordTime;
        return ResultModelFactory.Success<DateTime>(latestTime ?? new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
    }

    protected override Task OnUpdatedAsync(IEnumerable<AiChatMessageRecordEntity> data)
    {
        return aiChatRecordHubSyncManager.SyncAsync(
            new AiChatRecordHubTransferModel(data, HubTransferType.Add));
    }
}