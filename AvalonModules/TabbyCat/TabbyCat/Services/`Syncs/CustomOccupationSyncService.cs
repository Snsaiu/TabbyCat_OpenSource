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

[Register<ICustomOccupationSyncService>(ServiceLifetime.Singleton)]
public sealed class CustomOccupationSyncService(
    ICustomOccupationHubSyncManager customOccupationSyncManager,
    ICustomAssistantOccupationService customAssistantOccupationService
)
    : IncrementSyncServiceBase<IEnumerable<CustomAssistantOccupationEntity>, ICustomAssistantOccupationService,
            CustomAssistantOccupationEntity, Guid>(customAssistantOccupationService),
        ICustomOccupationSyncService
{
    protected override string QueryLatestVersionUrl =>
        $"/api/app/tabby-cat-custom-assistant-occupation/query-latest-occupation-assistant?email={User.Email}";

    protected override string SyncUrl => "/api/app/tabby-cat-custom-assistant-occupation/download-occupation-assistant";
    protected override string UploadUrl => "/api/app/tabby-cat-custom-assistant-occupation/upload-occupation-assistant";

    protected override async Task<IResultModel<bool>> UpdateLocalDbAsync(
        IEnumerable<CustomAssistantOccupationEntity> data)
    {
        foreach (var item in data) item.Email = User.Email;

        var result = await customAssistantOccupationService.AddRangeAsync(data);
        if (result)
        {
            return ResultModelFactory.Success<bool>(true);
        }
        else
        {
            Logger.LogError("自定义联系人配置远程数据更新到本地失败！");
            return ResultModelFactory.Error<bool>(AppResources.AnErrorOccurred);
        }
    }

    protected override Task UploadNewVersionToRemoteAsync(IEnumerable<CustomAssistantOccupationEntity> data)
    {
        return UploadRemoteAsync(data);
    }

    protected override Task OnUpdatedAsync(IEnumerable<CustomAssistantOccupationEntity> data)
    {
        return customOccupationSyncManager.SyncAsync(
            new CustomOccupationSettingHubTransferModel(data, HubTransferType.Update));
    }
}