using System.Threading;
using Duende.IdentityModel.Client;
using Duende.IdentityModel.OidcClient.Browser;
using Xamarin.Essentials;

namespace TabbyCat.Extensions;

public class AuthSystemBrowser:IBrowser
{
    public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
    {
        try
        {
            
            var result = await WebAuthenticator.AuthenticateAsync(
                new Uri(options.StartUrl),
                new Uri(options.EndUrl));

            var url = new RequestUrl("https://admin.yyan.cc/signin-oidc")
                .Create(new Parameters(result.Properties));

            return new BrowserResult
            {
                Response = url,
                ResultType = BrowserResultType.Success,
            };
        }
        catch (TaskCanceledException)
        {
            return new BrowserResult
            {
                ResultType = BrowserResultType.UserCancel
            };
        }
    }
}