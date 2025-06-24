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

[Register<IAiTemplateSettingSyncService>(ServiceLifetime.Singleton)]
public class AiTemplateSettingAsyncService(
    IAiTemplateSettingHubSyncManager aiTemplateSettingSyncManager,
    IAiTemplateSettingService aiTemplateSettingService)
    : IncrementSyncServiceBase<IEnumerable<AiTemplateSettingEntity>, IAiTemplateSettingService, AiTemplateSettingEntity,
        Guid>(aiTemplateSettingService), IAiTemplateSettingSyncService
{
    protected override string QueryLatestVersionUrl =>
        $"/api/app/tabby-cat-ai-template-setting/query-latest-ai-template-setting-version-setting?email={User.Email}";

    protected override string SyncUrl => "/api/app/tabby-cat-ai-template-setting/download-ai-template-setting";
    protected override string UploadUrl => "/api/app/tabby-cat-ai-template-setting/upload-ai-template-setting";

    protected override async Task<IResultModel<bool>> UpdateLocalDbAsync(IEnumerable<AiTemplateSettingEntity> data)
    {
        foreach (var item in data) item.Email = User.Email;

        var result = await aiTemplateSettingService.AddRangeAsync(data);
        if (result)
        {
            return ResultModelFactory.Success<bool>(true);
        }
        else
        {
            Logger.LogError("Ai配置远程数据更新到本地失败！");
            return ResultModelFactory.Error<bool>(AppResources.AnErrorOccurred);
        }
    }

    protected override Task UploadNewVersionToRemoteAsync(IEnumerable<AiTemplateSettingEntity> data)
    {
        return UploadRemoteAsync(data);
    }

    protected override Task OnUpdatedAsync(IEnumerable<AiTemplateSettingEntity> data)
    {
        return aiTemplateSettingSyncManager.SyncAsync(
            new AiTemplateSettingHubTransferModel(data, HubTransferType.Update));
    }
}