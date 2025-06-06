using System.Threading;
using Duende.IdentityModel.Client;
using Duende.IdentityModel.OidcClient.Browser;
using Xamarin.Essentials;

namespace TabbyCat.Extensions;

public class MobileAuthSystemBrowser : IBrowser
{
    public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
    {
        try
        {

            var result =  await WebAuthenticator.AuthenticateAsync(
                new(options.StartUrl),
                new("tabbycat://"));

            var query = string.Join("&", result.Properties
                .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));

            var finalUrl = $"{options.EndUrl}?{query}";

            return new BrowserResult
            {
                Response = finalUrl,
                ResultType = BrowserResultType.Success,
            };
        }
        catch (Exception exception)
        {
            return new BrowserResult
            {
                ResultType = BrowserResultType.UserCancel,
                Error = exception.Message
            };
        }
    }
}