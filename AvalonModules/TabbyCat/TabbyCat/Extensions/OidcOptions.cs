using Duende.IdentityModel.OidcClient;

namespace TabbyCat.Extensions;

public static class OidcOptions
{
    private static string redirectUri = string.Empty;
    private static string logoutRedirectUri = string.Empty;
    private static string clientId = string.Empty;

    public static OidcClientOptions GetOptions()
    {
        if (OperatingSystem.IsWindows() || OperatingSystem.IsMacOS() || OperatingSystem.IsLinux())
        {
            clientId = "YouYan_Application";
            redirectUri = "http://localhost:6578/signin-oidc";
            logoutRedirectUri = "http://localhost:6578/signout-callback-oidc";
        }
        else if (OperatingSystem.IsAndroid())
        {
            clientId = "YouYan_Android_Application";
            redirectUri = "tabbycat://signin-oidc";
        }

        var options = new OidcClientOptions()
        {
            Authority = "https://auth.yyan.cc",
            ClientId = clientId,
            RedirectUri = redirectUri,
            Scope = "email phone offline_access Admin address profile roles",
            PostLogoutRedirectUri = logoutRedirectUri,

        };
        if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS())
            options.Browser = new MobileAuthSystemBrowser();
        else
            options.Browser = new AvaloniaSystemBrowser(6578);
        return options;
    }
}