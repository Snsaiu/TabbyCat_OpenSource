using System.Net.Http;
using System.Threading;
using Duende.IdentityModel.OidcClient;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models.Users;

namespace TabbyCat.Extensions;

public class TokenHandler : DelegatingHandler
{
    private readonly IUser _user;
    private readonly OidcClient _oidcClient;
    private readonly ILoginUserService _userService;


    public TokenHandler(IUser user, OidcClient oidcClient, ILoginUserService userService)
    {
        _user = user;
        _oidcClient = oidcClient;
        _userService = userService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (_user.AccessTokenExpiration is null)
            return await base.SendAsync(request, cancellationToken);

        if (_user.AccessTokenExpiration < DateTime.UtcNow && !string.IsNullOrEmpty(_user.RefreshToken))
        {
            var refreshTokenResult = await _oidcClient.RefreshTokenAsync(_user.RefreshToken);
            if (refreshTokenResult.IsError) throw new(refreshTokenResult.ErrorDescription);
            _user.AccessTokenExpiration = refreshTokenResult.AccessTokenExpiration;
            _user.RefreshToken = refreshTokenResult.RefreshToken;
            _user.AccessToken = refreshTokenResult.AccessToken;
            _userService.Set(_user as LoginUserModel);
        }

        if (!string.IsNullOrEmpty(_user.AccessToken))
            request.Headers.Authorization = new("Bearer", _user.AccessToken);

        return await base.SendAsync(request, cancellationToken);
    }
}