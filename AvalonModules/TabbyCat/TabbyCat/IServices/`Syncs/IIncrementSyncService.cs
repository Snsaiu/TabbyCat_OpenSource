using TabbyCat.Repository.Entities;
using YouYan.Hamster.ResultModels;

namespace TabbyCat.IServices;

/// <summary>
/// 增量同步数据接口
/// </summary>
/// <typeparam name="T">同步的数据类型</typeparam>
public interface IIncrementSyncService<T> : ISyncService<T>
{
    /// <summary>
    /// 查询最新的版本号
    /// </summary>
    /// <returns>返回服务端最新的版本号</returns>
    Task<IResultModel<int>> QueryLatestVersionAsync();
}