using AirTransfer.Interfaces;

namespace AirTransfer
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();


            SetupTrayIcon();
        }

        //        protected override void OnHandlerChanged()
        //        {
        //#if WINDOWS
        //    Microsoft.UI.Xaml.Window window =
        // (Microsoft.UI.Xaml.Window)App.Current.Windows.First<Window>().Handler.PlatformView;
        //            MauiAppExtension.HideTaskBar(window);
        //#endif
        //        }

        private void SetupTrayIcon()
        {
#if MACCATALYST || WINDOWS
            var trayService = Extensions.ServiceProvider.RequestService<ITrayService>();
            if (trayService != null)
            {
                trayService.Initialize();
                trayService.ClickHandler = () => { };
            }
#endif
        }


    }
}