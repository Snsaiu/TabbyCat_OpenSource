using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Duende.IdentityModel.OidcClient;
using FluentAvalonia.UI.Windowing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TabbyCat.Enums;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models.Users;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels;
using TuDog.Bootstrap;
using TuDog.Interfaces.IDialogServers;

namespace TabbyCat.Views
{
    public partial class MainWindow : AppWindow
    {
        private IDialogServer dialogService = TuDogApplication.ServiceProvider.GetRequiredService<IDialogServer>();

        private ILoginUserService userService = TuDogApplication.ServiceProvider.GetRequiredService<ILoginUserService>();

        private IUser user = TuDogApplication.ServiceProvider.GetRequiredService<IUser>();

        private OidcClient oidcClient = TuDogApplication.ServiceProvider.GetRequiredService<OidcClient>();

        private ILogger<MainWindow> logger = TuDogApplication.ServiceProvider.GetRequiredService<ILogger<MainWindow>>();

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            if (OperatingSystem.IsWindows())
            {
                this.TitleBar.ExtendsContentIntoTitleBar = true;
                this.TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
                Closing += MainWindow_Closing;
            }
        }

        private void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
        {
            OnClose(e);
        }

        private  void OnClose(object? sender, RoutedEventArgs e)
        {
            OnClose(null);
        }

        private async void OnClose(object? e)
        {
            if (OperatingSystem.IsWindows())
            {
                if(e is WindowClosingEventArgs arg)
                    arg.Cancel = true;
                else
                {
                    logger.LogError("window下关闭程序中的参数e不是{0}",typeof(WindowClosingEventArgs));
                   await dialogService.ShowMessageDialogAsync(AppResources.AnErrorOccurred, AppResources.Warning,
                        AppResources.Ok);
                }
            }

            var closeService = TuDogApplication.ServiceProvider.GetRequiredService<ICloseWindowStateService>();
            var state = closeService.Get();
            if (state == WindowCloseState.Closed)
            {
                if (OperatingSystem.IsWindows())
                {
                    if (e is WindowClosingEventArgs arg)
                    {
                        arg.Cancel = false;
                        Environment.Exit(0);
                        return;
                    }
                    else
                    {
                        logger.LogError("window下关闭程序中的参数e不是{0}",typeof(WindowClosingEventArgs));
                        await dialogService.ShowMessageDialogAsync(AppResources.AnErrorOccurred, AppResources.Warning,
                            AppResources.Ok);
                    }
                }

                this.Close();
            }
            else if (state == WindowCloseState.Minimized)
            {
                this.WindowState = WindowState.Minimized;
            }
            else
            {
                var dialogResult =
                    await dialogService.ShowConfirmDialogAsync(AppResources.DoYouWantToCloseTheProgram,
                        AppResources.Message, AppResources.CloseProgram,
                        AppResources.Minimize);
                if (dialogResult)
                {
                    if (OperatingSystem.IsWindows())
                    {
                        if (e is WindowClosingEventArgs arg)
                        {
                            arg.Cancel = false;
                            Environment.Exit(0);
                            return;
                        }
                        else
                        {
                            logger.LogError("window下关闭程序中的参数e不是{0}",typeof(WindowClosingEventArgs));
                            await dialogService.ShowMessageDialogAsync(AppResources.AnErrorOccurred, AppResources.Warning,
                                AppResources.Ok);
                        }
                    }

                    this.Close();
                }
                else
                {
                    WindowState = WindowState.Minimized;
                }
            }
        }

        private void OnMin(object? sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }


        private async void Login(object? sender, RoutedEventArgs e)
        {
            await LoginAsync();
        }

        private async Task LoginAsync()
        {
            loginButton.IsEnabled = false;
            var result = await OidcLogin();
            if (result is null)
            {
                await this.dialogService.ShowMessageDialogAsync(AppResources.LoginErrorTryAgain, AppResources.Warning, AppResources.Ok);
                this.loginButton.IsEnabled = true;
                this.userButton.IsVisible = true;
                this.userButton.IsVisible = false;
                return;
            }
            
            // 写入User中
            var email = WriteUserInfo(result);
            if (email is null)
            {
                await this.dialogService.ShowMessageDialogAsync(AppResources.LoginErrorEmailNullTryAagin, AppResources.Warning, AppResources.Ok);
                this.loginButton.IsEnabled = true;
                this.userButton.IsVisible = true;
                this.userButton.IsVisible = false;
                return;
            }
            this.loginButton.IsVisible = false;
            loginButton.IsEnabled = true;
            this.userButton.IsVisible = true;
            this.userButton.Content = email;
        }

        private async Task<LoginResult?> OidcLogin()
        {
            try
            {
                var result = await oidcClient.LoginAsync(new() { BrowserTimeout = 60 });
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
            user.ResetData(newUser);
            userService.Set(newUser);
            return email;
        }


        private async void Control_OnLoaded(object? sender, RoutedEventArgs e)
        {
            if (!user.LoginSuccess())
            {
                loginButton.IsVisible = true;
                userButton.IsVisible = false;
                if (user.AccessTokenExpiration < DateTimeOffset.Now)
                {
                    logger.LogInformation("登录时间已经过期,过期时间{0}。",user.AccessTokenExpiration);
                    
                    if (await dialogService.ShowConfirmDialogAsync(AppResources.DoYouWantToRelogin,
                            AppResources.Message, AppResources.Ok, AppResources.Cancel))
                    {
                        logger.LogInformation("确定重新登录。");
                        
                        await LoginAsync();
                    }
                    else
                    {
                        logger.LogInformation("放弃登录。");
                        
                        user.Clear();
                        userService.SetNull();
                        
                        logger.LogInformation("清空本地登录信息。");
                    }
                }
            }
            else
            {
                loginButton.IsVisible = false;
                userButton.Content = user.Email;
                userButton.IsVisible = true;
                logger.LogInformation("登录时间未过期，自动登录。");
            }
        }

        private async void Logout(object? sender, RoutedEventArgs e)
        {
            var result =
                await dialogService.ShowDialogAsync<LogoutViewModel, LogoutOptionModel>(AppResources.Logout,
                    AppResources.Logout,AppResources.Cancel);
            if (result is { Ok: true })
            {
                this.userButton.IsEnabled = false;
                var logoutResult = await oidcClient.LogoutAsync();

                if (logoutResult.IsError)
                {
                    await dialogService.ShowMessageDialogAsync($"{AppResources.LogoutFailed}:{logoutResult.Error}",
                        AppResources.Message, AppResources.Ok);
                    return;
                }

                userService.SetNull();
                await dialogService.ShowMessageDialogAsync(AppResources.AppWillClose, AppResources.Warning,
                    AppResources.Ok);
                Environment.Exit(0);
            }
        }
    }
}