using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Duende.IdentityModel.OidcClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TabbyCat.Enums;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models;
using TabbyCat.Models.Users;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels.Bases;
using TuDog.Bootstrap;
using TuDog.Interfaces.IDialogServers;
using TuDog.Interfaces.RegionManagers;

namespace TabbyCat.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{
    private IDialogServer dialogService = TuDogApplication.ServiceProvider.GetRequiredService<IDialogServer>();

    private ILoginUserService userService = TuDogApplication.ServiceProvider.GetRequiredService<ILoginUserService>();

    private OidcClient oidcClient = TuDogApplication.ServiceProvider.GetRequiredService<OidcClient>();

    private ILogger<MainWindowViewModel> logger =
        TuDogApplication.ServiceProvider.GetRequiredService<ILogger<MainWindowViewModel>>();

    private IAiTemplateSettingService _aiTemplateSettingService =
        TuDogApplication.ServiceProvider.GetRequiredService<IAiTemplateSettingService>();

    private IAiTemplateSettingHubSyncManager aiTemplateSettingSyncManager =
        TuDogApplication.ServiceProvider.GetRequiredService<IAiTemplateSettingHubSyncManager>();

    private IRegionManager _regionManager { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IRegionManager>();

    [ObservableProperty] private IBackgroundImageConfig _backgroundImageConfig =
        TuDogApplication.ServiceProvider.GetRequiredService<IBackgroundImageConfig>();

    [ObservableProperty] private bool isLogined;

    [ObservableProperty] private IUser currentUser = TuDogApplication.ServiceProvider.GetRequiredService<IUser>();


    protected IAiTemplateSettingHubSyncManager AiTemplateSettingAsyncManager { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IAiTemplateSettingHubSyncManager>();

    protected IAiTemplateSettingSyncService AiTemplateSettingSyncService { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IAiTemplateSettingSyncService>();

    protected ICustomOccupationHubSyncManager CustomOccupationManager { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<ICustomOccupationHubSyncManager>();

    protected ICustomOccupationSyncService CustomOccupationSyncService { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<ICustomOccupationSyncService>();


    protected IChatSessionHubSyncManager ChatSessionSyncManager { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IChatSessionHubSyncManager>();

    protected IChatSessionSyncService ChatSessionSyncService { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IChatSessionSyncService>();

    protected IAiChatRecordHubSyncManager ChatRecordSyncManager =
        TuDogApplication.ServiceProvider.GetRequiredService<IAiChatRecordHubSyncManager>();

    protected IAiChatRecordSyncService ChatRecordSyncService =
        TuDogApplication.ServiceProvider.GetRequiredService<IAiChatRecordSyncService>();


    protected override async Task OnLoaded()
    {
        if (!CurrentUser.LoginSuccess())
        {
            IsLogined = false;
            _regionManager.AddToRegion<MainViewModel>("mainContainer");
            if (CurrentUser.AccessTokenExpiration < DateTimeOffset.Now)
            {
                logger.LogInformation("登录时间已经过期,过期时间{0}。", CurrentUser.AccessTokenExpiration);

                if (await dialogService.ShowConfirmDialogAsync(AppResources.DoYouWantToRelogin,
                        AppResources.Message, AppResources.Ok, AppResources.Cancel))
                {
                    logger.LogInformation("确定重新登录。");

                    await Login();
                }
                else
                {
                    logger.LogInformation("放弃登录。");

                    CurrentUser.Clear();
                    userService.SetNull();

                    logger.LogDebug("清空本地登录信息。");
                }
            }
        }
        else
        {
            using var progress =
                DialogServer.ShowProgressDialog(AppResources.DataSynchronization, AppResources.Preparing);

            var hubService = TuDogApplication.ServiceProvider.GetRequiredService<IHubService>();
            await hubService.StartAsync();

            progress.Progress(AppResources.DataSynchronization, AppResources.SynchronizeAIConfigurationInformation);
            await AiTemplateSettingAsyncManager.InitializeAsync();
            await AiTemplateSettingSyncService.SyncAsync();

            progress.Progress(AppResources.DataSynchronization, AppResources.SyncContacts);
            await CustomOccupationManager.InitializeAsync();
            await CustomOccupationSyncService.SyncAsync();

            progress.Progress(AppResources.DataSynchronization, AppResources.SynchronousChatSession);
            await ChatSessionSyncManager.InitializeAsync();
            await ChatSessionSyncService.SyncAsync();

            progress.Progress(AppResources.DataSynchronization, AppResources.SynchronizeChatHistory);
            await ChatRecordSyncManager.InitializeAsync();
            await ChatRecordSyncService.SyncAsync();

            _regionManager.AddToRegion<MainViewModel>("mainContainer");
            IsLogined = true;
        }
    }

    [RelayCommand]
    private async Task Login()
    {
        var result = await OidcLogin();
        if (result is null)
        {
            await dialogService.ShowConfirmDialogAsync(AppResources.LoginErrorTryAgain, AppResources.Warning,
                AppResources.Ok, string.Empty);
            IsLogined = false;
            return;
        }

        // 写入User中
        var email = WriteUserInfo(result);
        if (email is null)
        {
            await dialogService.ShowMessageDialogAsync(AppResources.LoginErrorEmailNullTryAagin, AppResources.Warning,
                AppResources.Ok);
            IsLogined = false;
            return;
        }

        if (await dialogService.ShowConfirmDialogAsync(AppResources.RebootToCompleteLogin, AppResources.Warning,
                AppResources.Ok, string.Empty)) Environment.Exit(0);

        // // 查看是否有默认的ai模型
        // await SetDefaultAiModel();
        // IsLogined = true;
    }

    private async Task SetDefaultAiModel()
    {
        if (!(await _aiTemplateSettingService.QueryAsync(x => x.IsDefault)).Any())
        {
            var aiModel = new TabbyCatAiModel();
            await _aiTemplateSettingService.AddAsync(new AiTemplateSettingEntity
            {
                IsDefault = true, Template = JsonConvert.SerializeObject(aiModel), Email = CurrentUser.Email,
                Key = Guid.NewGuid(), Provider = AiModelType.TabbyCatAi, CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            });
        }
    }

    [RelayCommand]
    private async Task Logout()
    {
        var result =
            await dialogService.ShowDialogAsync<LogoutViewModel, bool, LogoutOptionModel>(AppResources.Logout,
                AppResources.Logout, AppResources.Cancel);
        if (result is { Ok: true })
        {
            IsLogined = false;
            // var logoutResult = await oidcClient.LogoutAsync();
            //
            // if (logoutResult.IsError)
            // {
            //     await dialogService.ShowMessageDialogAsync($"{AppResources.LogoutFailed}:{logoutResult.Error}",
            //         AppResources.Message, AppResources.Ok);
            //     return;
            // }
            userService.SetNull();
            await dialogService.ShowMessageDialogAsync(AppResources.AppWillClose, AppResources.Warning,
                AppResources.Ok);
            Environment.Exit(0);
        }
    }

    private string? WriteUserInfo(LoginResult? result)
    {
        if (result is null) return null;

        var claims = result.User.Claims;
        var email = claims.FirstOrDefault(x => x.Type == "email")?.Value;
        var phone = claims.FirstOrDefault(x => x.Type == "phone_number")?.Value;
        var acccessToken = result.AccessToken;
        var accessTokenExpiration = result.AccessTokenExpiration;
        var refreshToken = result.RefreshToken;

        if (string.IsNullOrEmpty(email))
        {
            logger.LogError("邮箱不能为空，但实际为空！");
            return null;
        }


        var newUser = new LoginUserModel(email, phone, string.Empty, acccessToken, accessTokenExpiration, Sex.Man,
            refreshToken);
        CurrentUser.ResetData(newUser);
        userService.Set(newUser);
        return email;
    }


    private async Task<LoginResult?> OidcLogin()
    {
        try
        {
            var result = await oidcClient.LoginAsync(new LoginRequest { BrowserTimeout = 30 });
            if (result.IsError)
            {
                await dialogService.ShowMessageDialogAsync($"{AppResources.LoginFailed}: {result.Error}",
                    AppResources.Message, AppResources.Ok);
                return result;
            }

            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e, "登录错误");
            return null;
        }
    }
}