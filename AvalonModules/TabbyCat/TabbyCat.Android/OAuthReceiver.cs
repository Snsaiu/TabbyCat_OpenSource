using Android.App;
using Android.Content;
using System;

namespace TabbyCat.Android;

[BroadcastReceiver(Enabled = true, Exported = false)]
[IntentFilter(new[] { "com.tabbycat.OAUTH_CALLBACK" })]
public class OAuthReceiver : BroadcastReceiver
{
    public static Action<string>? OnCodeReceived;

    public override void OnReceive(Context context, Intent intent)
    {
        var code = intent.GetStringExtra("code");
        if (!string.IsNullOrEmpty(code)) OnCodeReceived?.Invoke(code);
    }
}