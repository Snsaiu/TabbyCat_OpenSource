using TabbyCat.Enums;

namespace TabbyCat.IServices;

public interface IUser
{
    string? PhoneNumber { get; set; }
    string Email { get; set; }
    string? Nickname { get; set; }
    string AccessToken { get; set; }
    DateTimeOffset? AccessTokenExpiration { get; set; }

    string RefreshToken { get; set; }
    Sex Sex { get; set; }

    bool LoginSuccess();

    public void ResetData(IUser user)
    {
        PhoneNumber = user.PhoneNumber;
        Email = user.Email;
        Nickname = user.Nickname;
        AccessToken = user.AccessToken;
        AccessTokenExpiration = user.AccessTokenExpiration;
        RefreshToken = user.RefreshToken;
        Sex = user.Sex;
    }

    public void Clear()
    {
        PhoneNumber = string.Empty;
        Email = string.Empty;
        Nickname = string.Empty;
        AccessToken = string.Empty;
        AccessTokenExpiration = null;
        RefreshToken = string.Empty;
        AccessTokenExpiration = null;
    }
}