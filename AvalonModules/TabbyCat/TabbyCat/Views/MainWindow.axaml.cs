using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Duende.IdentityModel.OidcClient;
using FluentAvalonia.UI.Windowing;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.Components.ViewModels;
using TabbyCat.Controls;
using TabbyCat.Enums;
using TabbyCat.Extensions;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models.Users;
using TabbyCat.Service.AiServices;
using TabbyCat.Service.RunningHubServices;
using TabbyCat.ViewModels;
using TuDog.Bootstrap;
using TuDog.Interfaces.IDialogServers;

namespace TabbyCat.Views
{
    public partial class MainWindow : AppWindow
    {
        private IDialogServer dialogService = TuDogApplication.ServiceProvider.GetService<IDialogServer>();

        private ILoginUserService userService = TuDogApplication.ServiceProvider.GetService<ILoginUserService>();

        private IUser user = TuDogApplication.ServiceProvider.GetService<IUser>();

        private OidcClient oidcClient = TuDogApplication.ServiceProvider.GetService<OidcClient>();

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

        private async void OnClose(object? sender, RoutedEventArgs e)
        {
            OnClose(null);
        }

        private async void OnClose(object? e)
        {
            if (OperatingSystem.IsWindows())
            {
                (e as WindowClosingEventArgs).Cancel = true;
            }

            var closeService = TuDogApplication.ServiceProvider.GetService<ICloseWindowStateService>();
            var state = closeService.Get();
            if (state == WindowCloseState.Closed)
            {
                if (OperatingSystem.IsWindows())
                {
                    (e as WindowClosingEventArgs).Cancel = false;
                    Environment.Exit(0);
                    return;
                }

                this.Close();
            }
            else if (state == WindowCloseState.Minimized)
            {
                this.WindowState = WindowState.Minimized;
            }
            else
            {
                var dialogService = TuDogApplication.ServiceProvider.GetService<IDialogServer>();
                var dialogResult = await dialogService.ShowConfirmDialogAsync("是否要关闭程序?", "消息", "关闭", "最小化");
                if (dialogResult)
                {
                    if (OperatingSystem.IsWindows())
                    {
                        (e as WindowClosingEventArgs).Cancel = false;
                        Environment.Exit(0);
                        return;
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
            // 写入User中
            var email = WriteUserInfo(result);
            this.loginButton.IsVisible = false;
            loginButton.IsEnabled = true;

            this.userButton.IsVisible = true;
            this.userButton.Content = email;
        }

        private async Task<LoginResult> OidcLogin()
        {
            var result = await oidcClient.LoginAsync(new() { BrowserTimeout = 60 });
            if (result.IsError)
            {
                await dialogService.ShowMessageDialogAsync($"登陆失败: {result.Error}");
                return result;
            }


            return result;
        }

        private string WriteUserInfo(LoginResult result)
        {
            var claims = result.User.Claims;
            var email = claims.FirstOrDefault(x => x.Type == "email")?.Value;
            var phone = claims.FirstOrDefault(x => x.Type == "phone_number")?.Value;
            var acccessToken = result.AccessToken;
            var accessTokenExpiration = result.AccessTokenExpiration;
            var refreshToken = result.RefreshToken;

            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));

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
                    if (await dialogService.ShowConfirmDialogAsync("账号需要重新登录，确定要重新登录吗？"))
                    {
                        await LoginAsync();
                    }
                    else
                    {
                        user.Clear();
                        userService.SetNull();
                    }
                }
            }
            else
            {
                loginButton.IsVisible = false;
                userButton.Content = user.Email;
                userButton.IsVisible = true;
            }
        }

        private async void Logout(object? sender, RoutedEventArgs e)
        {
            var result = await dialogService.ShowDialogAsync<LogoutViewModel, LogoutOptionModel>("登出", "登出");
            if (result is { Ok: true })
            {
                var logoutResult = await oidcClient.LogoutAsync();

                if (logoutResult.IsError)
                {
                    await dialogService.ShowMessageDialogAsync($"登出失败:{logoutResult.Error}");
                    return;
                }

                userService.SetNull();
                await dialogService.ShowMessageDialogAsync("软件即将关闭！", "警告");
                Environment.Exit(0);
            }
        }
    }
}