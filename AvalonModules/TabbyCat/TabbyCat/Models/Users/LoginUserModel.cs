using Microsoft.Extensions.DependencyInjection;
using TabbyCat.Enums;
using TabbyCat.IServices;
using TuDog.IocAttribute;

namespace TabbyCat.Models.Users;


[Register<IUser>(ServiceLifetime.Singleton)]
public sealed class LoginUserModel : IUser
{
    public LoginUserModel()
    {
    }

    public LoginUserModel(string email, string? phoneNumber, string? nickname, string accessToken,
        DateTimeOffset accessTokenExpiration, Sex sex, string refreshToken)
    {
        PhoneNumber = phoneNumber;
        Email = email;
        Nickname = nickname;
        AccessToken = accessToken;
        Sex = sex;
        AccessTokenExpiration=accessTokenExpiration;
        RefreshToken = refreshToken;
    }


    public string? PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string? Nickname { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;

    public DateTimeOffset? AccessTokenExpiration { get; set; }
    public string RefreshToken { get; set; } = string.Empty;

    public Sex Sex { get; set; }

    public bool LoginSuccess()
    {
        if (string.IsNullOrEmpty(AccessToken) || string.IsNullOrEmpty(RefreshToken) || AccessTokenExpiration is null ||
            string.IsNullOrEmpty(Email))
            return false;

        return !(AccessTokenExpiration < DateTime.UtcNow);
    }
}