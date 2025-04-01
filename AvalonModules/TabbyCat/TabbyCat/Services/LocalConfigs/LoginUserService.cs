using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models.Users;
using TuDog.Interfaces.PreferenceServices.Impl;
using TuDog.IocAttribute;

namespace TabbyCat.Services.LocalConfigs;

[Register<ILoginUserService>]
public sealed class LoginUserService : LocalConfigService<LoginUserModel>, ILoginUserService
{
    public override string Key { get; } = "loginUser";

    public override LoginUserModel Default { get; } = default;
}