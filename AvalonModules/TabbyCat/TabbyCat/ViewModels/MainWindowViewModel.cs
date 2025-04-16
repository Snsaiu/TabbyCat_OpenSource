using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Duende.IdentityModel.OidcClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TabbyCat.Enums;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models.Users;
using TabbyCat.Shared.Languages;
using TuDog.Bootstrap;
using TuDog.Interfaces.IDialogServers;
using TuDog.Interfaces.RegionManagers;

namespace TabbyCat.ViewModels;

public partial class MainWindowViewModel:ViewModelBase
{

    private IDialogServer dialogService = TuDogApplication.ServiceProvider.GetRequiredService<IDialogServer>();

    private ILoginUserService userService = TuDogApplication.ServiceProvider.GetRequiredService<ILoginUserService>();
    
    private OidcClient oidcClient = TuDogApplication.ServiceProvider.GetRequiredService<OidcClient>();

    private ILogger<MainWindowViewModel> logger = TuDogApplication.ServiceProvider.GetRequiredService<ILogger<MainWindowViewModel>>();
    
    private IRegionManager _regionManager { get; }=TuDogApplication.ServiceProvider.GetRequiredService<IRegionManager>();

    [ObservableProperty]
    private bool isLogined;
    
    [ObservableProperty]
    private IUser currentUser = TuDogApplication.ServiceProvider.GetRequiredService<IUser>();

    protected async override Task OnLoaded()
    {

        if (!CurrentUser.LoginSuccess())
        {
            IsLogined = false;
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

                    logger.LogInformation("清空本地登录信息。");
                }
            }
        }
        else
        {
            IsLogined = true;  
        }

    
        _regionManager.AddToRegion<MainViewModel>("mainContainer");
    }

    [RelayCommand]
    private async Task Login()
    {
        var result = await OidcLogin();
        if (result is null)
        {
            await this.dialogService.ShowMessageDialogAsync(AppResources.LoginErrorTryAgain, AppResources.Warning, AppResources.Ok);
            IsLogined = false;
            return;
        }
        // 写入User中
        var email = WriteUserInfo(result);
        if (email is null)
        {
            await this.dialogService.ShowMessageDialogAsync(AppResources.LoginErrorEmailNullTryAagin, AppResources.Warning, AppResources.Ok);
            IsLogined = false;
            return;
        }

        IsLogined = true;
    }

    [RelayCommand]
    private async Task Logout()
    {
        var result =
            await dialogService.ShowDialogAsync<LogoutViewModel, LogoutOptionModel>(AppResources.Logout,
                AppResources.Logout,AppResources.Cancel);
        if (result is { Ok: true })
        {
            this.IsLogined = false;
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
    
    private string? WriteUserInfo(LoginResult result)
    {
        var claims = result.User.Claims;
        var email = claims.FirstOrDefault(x => x.Type == "email")?.Value;
        var phone = claims.FirstOrDefault(x => x.Type == "phone_number")?.Value;
        var acccessToken = result.AccessToken;
        var accessTokenExpiration = result.AccessTokenExpiration;
        var refreshToken = result.RefreshToken;

        if (string.IsNullOrEmpty(email))
        {
            this.logger.LogError("邮箱不能为空，但实际为空！");
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
            var result = await oidcClient.LoginAsync(new() { BrowserTimeout = 10 });
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
            this.logger.LogError(e, "登录错误");
            return null;
        }
    }

    
}