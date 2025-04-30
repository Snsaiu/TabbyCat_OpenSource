using System.Net;
using System.Net.Http;
using System.Threading;
using Duende.IdentityModel.OidcClient;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models.Users;

namespace TabbyCat.Extensions;

public class TokenHandler(IUser user, OidcClient oidcClient, ILoginUserService userService)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {

        if (user.AccessTokenExpiration is null)
            return await base.SendAsync(request, cancellationToken);

        if (user.AccessTokenExpiration < DateTime.UtcNow && !string.IsNullOrEmpty(user.RefreshToken))
        {
            var refreshTokenResult = await oidcClient.RefreshTokenAsync(user.RefreshToken, cancellationToken: cancellationToken);
            if (refreshTokenResult.IsError) throw new(refreshTokenResult.ErrorDescription);
            user.AccessTokenExpiration = refreshTokenResult.AccessTokenExpiration;
            user.RefreshToken = refreshTokenResult.RefreshToken;
            user.AccessToken = refreshTokenResult.AccessToken;
            userService.Set((LoginUserModel)user);
        }

        if (!string.IsNullOrEmpty(user.AccessToken))
            request.Headers.Authorization = new("Bearer", user.AccessToken);

        return await base.SendAsync(request, cancellationToken);
    }
}