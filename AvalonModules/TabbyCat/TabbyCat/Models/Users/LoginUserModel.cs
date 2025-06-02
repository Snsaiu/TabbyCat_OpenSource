using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.Enums;
using TabbyCat.IServices;
using TuDog.Bootstrap;
using TuDog.IocAttribute;

namespace TabbyCat.Models.Users;


[Register<IUser>(ServiceLifetime.Singleton)]
public sealed partial class LoginUserModel : ModelBase, IUser
{
    public LoginUserModel()
    {
        IsLogin = LoginSuccess();
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


    [ObservableProperty] private string? phoneNumber = string.Empty;
    [ObservableProperty] private string email = string.Empty;

    [ObservableProperty] private string? nickname = string.Empty;
    [ObservableProperty] private string accessToken = string.Empty;

    [ObservableProperty] private DateTimeOffset? accessTokenExpiration;

    [ObservableProperty] private string refreshToken = string.Empty;

    [ObservableProperty] private Sex sex;

    public bool LoginSuccess()
    {
        if (string.IsNullOrEmpty(AccessToken) || string.IsNullOrEmpty(RefreshToken) || AccessTokenExpiration is null ||
            string.IsNullOrEmpty(Email))
            return false;

        return !(AccessTokenExpiration < DateTime.UtcNow);
    }
    
  
    
   

    partial void OnEmailChanged(string value)
    {
        IsLogin = !string.IsNullOrEmpty(Email);
    }
    [ObservableProperty]
    private bool _isLogin;
}