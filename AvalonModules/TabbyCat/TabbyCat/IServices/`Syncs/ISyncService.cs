using TabbyCat.Repository.Entities;
using YouYan.Hamster.ResultModels;

namespace TabbyCat.IServices;

/// <summary>
/// 同步接口
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISyncService<T>
{
    /// <summary>
    /// 同步
    /// </summary>
    /// <returns></returns>
    Task<IResultModel<bool>> SyncAsync();

    /// <summary>
    /// 将本地<see cref="T"/>数据同步到远程
    /// </summary>
    /// <param name="data">需要推送到远程的数据</param>
    /// <returns></returns>
    Task UploadRemoteAsync(T data);
}