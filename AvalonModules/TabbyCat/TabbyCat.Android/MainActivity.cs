using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;

using Avalonia;
using Avalonia.Android;
using Xamarin.Essentials;

namespace TabbyCat.Android
{
    [Activity(
        Label = "TabbyCat",
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
            Platform.Init(this, savedInstanceState);

            // OAuthReceiver.OnCodeReceived = code =>
            // {
            //     // 这里处理获取到的 code，比如发请求获取 token
            //     Console.WriteLine("OAuth 回调成功，code: " + code);
            // };

            Window.SetSoftInputMode(SoftInput.AdjustResize);

            // 确保不全屏
            Window.ClearFlags(WindowManagerFlags.Fullscreen);

            // 显示状态栏
            Window.DecorView.SystemUiVisibility = StatusBarVisibility.Visible;


        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnNewIntent(Intent intent, ComponentCaller caller)
        {
            base.OnNewIntent(intent, caller);
            if (intent?.Data != null)
            {
                // 你可以在这里处理回调，比如保存 token 等
            }
        }
    }
}