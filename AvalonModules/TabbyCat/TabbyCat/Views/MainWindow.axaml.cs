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

        
     
    }
}