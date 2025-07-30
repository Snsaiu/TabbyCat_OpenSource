using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
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
using Color = Avalonia.Media.Color;

namespace TabbyCat.Views;

public partial class MainWindow : AppWindow
{
    private IDialogServer dialogService = TuDogApplication.ServiceProvider.GetRequiredService<IDialogServer>();

    private ILogger<MainWindow> logger = TuDogApplication.ServiceProvider.GetRequiredService<ILogger<MainWindow>>();

    private ITopMostService topMostService = TuDogApplication.ServiceProvider.GetRequiredService<ITopMostService>();

    private IShowWindowTypeConfig showWindowTypeConfig =
        TuDogApplication.ServiceProvider.GetRequiredService<IShowWindowTypeConfig>();

    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        if (OperatingSystem.IsWindows())
        {
            TitleBar.ExtendsContentIntoTitleBar = true;
            TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
            Closing += MainWindow_Closing;
        }
    }

    private MainWindowViewModel vm;

    private Size windowBackSize;

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Size windowBackSize;
        if (OperatingSystem.IsWindows())
            windowBackSize = new Size(Width, Height);
        else
            windowBackSize = new Size(ClientSize.Width, ClientSize.Height);

        vm = DataContext as MainWindowViewModel;
        showWindowTypeConfig.ChangedCallBack += (type) =>
        {
            if (type == WindowsShowType.FloatingFrame)
            {
                Topmost = true;
                TitleBar.Height = 0;
                SizeToContent = SizeToContent.WidthAndHeight;
            }
            else
            {
                Topmost = topMostService.Get();
                SizeToContent = SizeToContent.Manual;
                Width = windowBackSize.Width;
                Height = windowBackSize.Height;
                TitleBar.Height = 32;
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        };
    }

    private void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        OnClose(e);
    }

    private void OnClose(object? sender, RoutedEventArgs e)
    {
        OnClose(null);
    }

    private async void OnClose(object? e)
    {
        if (OperatingSystem.IsWindows())
        {
            if (e is WindowClosingEventArgs arg)
            {
                arg.Cancel = true;
            }
            else
            {
                logger.LogError("window下关闭程序中的参数e不是{0}", typeof(WindowClosingEventArgs));
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
                    logger.LogError("window下关闭程序中的参数e不是{0}", typeof(WindowClosingEventArgs));
                    await dialogService.ShowMessageDialogAsync(AppResources.AnErrorOccurred, AppResources.Warning,
                        AppResources.Ok);
                }
            }

            Close();
        }
        else if (state == WindowCloseState.Minimized)
        {
            WindowState = WindowState.Minimized;
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
                        logger.LogError("window下关闭程序中的参数e不是{0}", typeof(WindowClosingEventArgs));
                        await dialogService.ShowMessageDialogAsync(AppResources.AnErrorOccurred, AppResources.Warning,
                            AppResources.Ok);
                    }
                }

                Close();
            }
            else
            {
                WindowState = WindowState.Minimized;
            }
        }
    }

    private void OnMin(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }
}