using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Xamarin.Essentials;

namespace TabbyCat.Android;

[Activity(NoHistory = true, Exported = true, LaunchMode = LaunchMode.SingleTop)]
[IntentFilter(
    new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataScheme = "tabbycat",
    DataHost = "signin-oidc")]
public class OAuthCallbackActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        var data = Intent?.Data;
        if (data != null)
        {
            var code = data.GetQueryParameter("code");

            // 发送广播，主线程监听处理
            var broadcast = new Intent("com.tabbycat.OAUTH_CALLBACK");
            broadcast.PutExtra("code", code);
            SendBroadcast(broadcast);
        }

        Finish(); // 自动关闭中间 Activity
    }
}