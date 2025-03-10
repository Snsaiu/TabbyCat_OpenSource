using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using FluentAvalonia.UI.Windowing;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.Controls;
using TabbyCat.Enums;
using TabbyCat.IServices.LocalConfigs;
using TuDog.Bootstrap;
using TuDog.Interfaces.IDialogServers;

namespace TabbyCat.Views
{
    public partial class MainWindow : AppWindow
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            if(OperatingSystem.IsWindows())
            {

                this.TitleBar.ExtendsContentIntoTitleBar = true;
                this.TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

                this.Closing+=MainWindow_Closing;
            }



        }

        private void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
        {
           OnClose(e);
        }

        private async  void OnClose(object? sender, RoutedEventArgs e)
        {
            OnClose(null);

        }

        private async void OnClose(object? e)
        {
            if(OperatingSystem.IsWindows())
            {
                (e as WindowClosingEventArgs).Cancel=true;
            }
            var closeService = TuDogApplication.ServiceProvider.GetService<ICloseWindowStateService>();
            var state = closeService.Get();
            if (state == WindowCloseState.Closed)
            {
                if (OperatingSystem.IsWindows())
                {
                    (e as WindowClosingEventArgs).Cancel=false;
                    Environment.Exit(0);
                    return;

                }
                this.Close();
            }
            else if (state== WindowCloseState.Minimized)
            {
                this.WindowState = WindowState.Minimized;
            }
            else
            {
                var dialogService = TuDogApplication.ServiceProvider.GetService<IDialogServer>();
                var dialogResult = await dialogService.ShowConfirmDialogAsync("是否要关闭程序?", "消息", "关闭", "最小化");
                if (dialogResult)
                {    if(OperatingSystem.IsWindows())
                    {
                        (e as WindowClosingEventArgs).Cancel=false;
                        Environment.Exit(0);
                        return;

                    }
                    this.Close();
                }
                else
                {
                    this.WindowState= WindowState.Minimized;
                }
            }
        }

        private void OnMin(object? sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}