using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

using Avalonia;
using Avalonia.Android;

namespace TabbyCat.Android
{
    [Activity(
        Label = "TabbyCat.Android",
        Theme = "@style/MyTheme.NoActionBar",
        Icon = "@drawable/icon",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
    public class MainActivity : AvaloniaMainActivity<App>
    {
        protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
        {
            //  Batteries_V2.Init(); // 使用 SQLitePCLRaw 自带的 e_sqlite3

            return base.CustomizeAppBuilder(builder)
                .WithInterFont();
        }

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Window.SetSoftInputMode(SoftInput.AdjustResize);
        }
    }
}
