using TabbyCat.App.Models;

using FantasyResultModel;
using FantasyResultModel.Impls;

using Newtonsoft.Json;

namespace TabbyCat.App.Interfaces.Impls.Configs;

public class ConfigUserService : IUserService
{
    public Task<ResultBase<bool>> SaveUserAsync(UserInfo user)
    {
        Preferences.Default.Set<string>("user", JsonConvert.SerializeObject(user));
        return Task.FromResult<ResultBase<bool>>(new SuccessResultModel<bool>(true));
    }

    public Task<ResultBase<UserInfo>> GetCurrentUserAsync()
    {
        var v = Preferences.Default.Get<string>("user", "");
        if (string.IsNullOrEmpty(v))
        {
            return Task.FromResult<ResultBase<UserInfo>>(new ErrorResultModel<UserInfo>("未发现用户信息"));
        }

        var user = JsonConvert.DeserializeObject<UserInfo>(v);

        return user is null
            ? Task.FromResult<ResultBase<UserInfo>>(new ErrorResultModel<UserInfo>("序列化用户失败"))
            : Task.FromResult<ResultBase<UserInfo>>(new SuccessResultModel<UserInfo>(user));
    }

    public Task<ResultBase<bool>> ClearUserAsync()
    {
        Preferences.Default.Set<string>("user", string.Empty);
        return Task.FromResult<ResultBase<bool>>(new SuccessResultModel<bool>(true));
    }
}