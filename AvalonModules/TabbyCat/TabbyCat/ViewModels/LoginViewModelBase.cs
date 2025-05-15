using CommunityToolkit.Mvvm.ComponentModel;
using Duende.IdentityModel.OidcClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TabbyCat.Enums;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models.Users;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels.Bases;
using TuDog.Bootstrap;
using TuDog.Interfaces.IDialogServers;

namespace TabbyCat.ViewModels;

public partial class LoginViewModelBase : ViewModelBase
{

    [ObservableProperty]
    private bool isLogined;

    [ObservableProperty]
    private IUser currentUser = TuDogApplication.ServiceProvider.GetRequiredService<IUser>();

    private ILogger<LoginViewModelBase> logger = TuDogApplication.ServiceProvider.GetRequiredService<ILogger<LoginViewModelBase>>();

    protected IDialogServer dialogService = TuDogApplication.ServiceProvider.GetRequiredService<IDialogServer>();

    protected ILoginUserService userService = TuDogApplication.ServiceProvider.GetRequiredService<ILoginUserService>();

    protected OidcClient oidcClient = TuDogApplication.ServiceProvider.GetRequiredService<OidcClient>();


    protected async Task LoginAsync()
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

    private async Task<LoginResult?> OidcLogin()
    {
        try
        {
            var result = await oidcClient.LoginAsync(new() { BrowserTimeout = 30 });
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

    protected async Task ValidateLoginAsync()
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

                    await LoginAsync();
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
    }
}