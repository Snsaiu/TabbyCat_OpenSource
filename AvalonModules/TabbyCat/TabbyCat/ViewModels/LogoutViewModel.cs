using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models.Users;
using TabbyCat.Service.AiServices;
using TabbyCat.Service.RunningHubServices;
using TabbyCat.Shared.Enums;
using TuDog.Bootstrap;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public sealed partial class LogoutViewModel(
    IAiChatMessageRecordService aiChatMessageRecordService,
    IAiChatSessionService aiChatSessionService,
    ICustomAssistantOccupationService customAssistantOccupationService,
    IAiMediaService runningHubService,
    IAiMediaResourceService runningHubResourceService,
    IAiMediaResultService runningHubResultService,
    ILogger<LogoutViewModel> logger,
    IAiTemplateSettingService aiTemplateSettingService) : DialogViewModelBaseAsync<bool, LogoutOptionModel>
{
    [ObservableProperty] private LogoutOptionModel model = new();

    public override async Task<LogoutOptionModel?> ConfirmAsync()
    {
        if (Model.ClearChats)
        {
            await aiChatMessageRecordService.DeleteRangeAsync(x => true);
            await aiChatSessionService.DeleteRangeAsync(x => true);
            await customAssistantOccupationService.DeleteRangeAsync(x => true);
            await runningHubService.DeleteRangeAsync(x => true);
            logger.LogDebug("清空聊天历史记录成功。");
        }

        if (Model.ClearImageResource)
        {
            runningHubResourceService.Set(runningHubResourceService.Default);
            await runningHubResultService.DeleteRangeAsync(x => true);
            logger.LogDebug("清空图片资源成功。");
        }

        if (Model.ClearAiApiKeys)
        {
            await aiTemplateSettingService.DeleteRangeAsync(x => true);
            logger.LogDebug("清空Ai API Key成功。");
        }

        return Model;
    }

    public override Task<LogoutOptionModel> CancelAsync()
    {
        return Task.FromResult<LogoutOptionModel>(null);
    }
}