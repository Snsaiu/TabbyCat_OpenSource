using CommunityToolkit.Mvvm.ComponentModel;
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
    IRunningHubService runningHubService,
    IRunningHubResourceService runningHubResourceService,
    IRunningHubResultService runningHubResultService,
    IAiTemplateSettingService aiTemplateSettingService) : DialogViewModelBase<LogoutOptionModel>
{
    [ObservableProperty] private LogoutOptionModel model = new();

    protected override async Task<LogoutOptionModel?> OnConfirmAsync()
    {
        var user = loginUserService.Get();
        if (model.ClearChats == true)
        {
            await aiChatMessageRecordService.DeleteRangeAsync(x => x.PhoneNumber == user.PhoneNumber);
            await aiChatSessionService.DeleteRangeAsync(x => x.PhoneNumber == user.PhoneNumber);
            await customAssistantOccupationService.DeleteRangeAsync(x => x.PhoneNumber == user.PhoneNumber);
            await runningHubService.DeleteRangeAsync(x => x.PhoneNumber == user.PhoneNumber);
        }

        if (model.ClearImageResource == true)
        {
            runningHubResourceService.Set(runningHubResourceService.Default);
            await runningHubResultService.DeleteRangeAsync(x => x.PhoneNumber == user.PhoneNumber);
        }

        if (model.ClearAiApiKeys == true)
            await aiTemplateSettingService.DeleteRangeAsync(x => x.PhoneNumber == user.PhoneNumber);

        return model;
    }
}