using Duende.IdentityModel.OidcClient;

namespace TabbyCat.Extensions;

public static class OidcOptions
{
    public static readonly string redirectUri = "http://localhost:6578/signin-oidc";
    public static readonly string logoutRedirectUri = "http://localhost:6578/signout-callback-oidc";

    public static OidcClientOptions GetOptions()
    {
        return new()
        {
            Authority = "https://auth.yyan.cc",
            ClientId = "YouYan_Application",
            RedirectUri = redirectUri,
            Scope = "email phone offline_access Admin address profile roles",
            PostLogoutRedirectUri = logoutRedirectUri,
            Browser = new AvaloniaSystemBrowser(6578)
        };
    }
}