using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models.Users;
using TabbyCat.Service.AiServices;
using TabbyCat.Service.RunningHubServices;
using TuDog.Bootstrap;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public sealed partial class LogoutViewModel(
    IAiChatMessageRecordService aiChatMessageRecordService,
    IAiChatSessionService aiChatSessionService,
    ICustomAssistantOccupationService customAssistantOccupationService,
    ILoginUserService loginUserService,
    IAiMediaService runningHubService,
    IAiMediaResourceService runningHubResourceService,
    IAiMediaResultService runningHubResultService,
    ILogger<LogoutViewModel> logger,
    IAiTemplateSettingService aiTemplateSettingService) : DialogViewModelBaseAsync<bool,LogoutOptionModel>
{
    [ObservableProperty] private LogoutOptionModel model = new();

    public override async Task<LogoutOptionModel?> ConfirmAsync()
    {
        var user = loginUserService.Get();
        if (Model.ClearChats)
        {
            await aiChatMessageRecordService.DeleteRangeAsync(x => x.Email == user.Email);
            await aiChatSessionService.DeleteRangeAsync(x => x.Email == user.Email);
            await customAssistantOccupationService.DeleteRangeAsync(x => x.Email == user.Email);
            await runningHubService.DeleteRangeAsync(x => x.Email == user.Email);
            logger.LogInformation("清空聊天历史记录成功。");
        }

        if (Model.ClearImageResource )
        {
            runningHubResourceService.Set(runningHubResourceService.Default);
            await runningHubResultService.DeleteRangeAsync(x => x.Email == user.Email);
            logger.LogInformation("清空图片资源成功。");
        }

        if (Model.ClearAiApiKeys)
        {
            await aiTemplateSettingService.DeleteRangeAsync(x => x.Email == user.Email);
            logger.LogInformation("清空Ai API Key成功。");
        }

        return Model;
    }

    public override Task<LogoutOptionModel> CancelAsync()
    {
        return null;
    }
}