using TabbyCat.App.Models;

using FantasyResultModel;

namespace TabbyCat.App.Interfaces;

public interface IUserService
{
    /// <summary>
    /// 保存用户信息
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<ResultBase<bool>> SaveUserAsync(UserInfo user);


    /// <summary>
    /// 获得当前用户
    /// </summary>
    /// <returns></returns>
    public Task<ResultBase<UserInfo>> GetCurrentUserAsync();

    /// <summary>
    /// 清空当前用户
    /// </summary>
    /// <returns></returns>
    public Task<ResultBase<bool>> ClearUserAsync();
}