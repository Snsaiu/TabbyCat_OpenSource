using Android.App;
using Android.Content;
using Android.Content.PM;

namespace TabbyCat.Android;

[Activity(NoHistory = true, Exported = true, LaunchMode = LaunchMode.SingleTop)]
[IntentFilter(
    new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataScheme = "tabbycat"
    )]
public class WebAuthenticatorCallbackActivity : Xamarin.Essentials.WebAuthenticatorCallbackActivity
{
    protected override void OnResume()
    {
        base.OnResume();

        Xamarin.Essentials.Platform.OnResume();
    }
}