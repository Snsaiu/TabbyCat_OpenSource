using System.Net.Http;
using Serilog;
using TabbyCat.Extensions;
using TabbyCat.Models;
using TabbyCat.Repository;
using TabbyCat.Repository.Entities;
using TuDog.Extensions;
using YouYan.Hamster.ResultModels;

namespace TabbyCat.IServices;

/// <summary>
/// 增量更新
/// </summary>
/// <param name="httpClientFactory"></param>
/// <param name="user"></param>
/// <param name="localService"></param>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TLocalService"></typeparam>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TPrimaryKey"></typeparam>
public abstract class IncrementSyncServiceBase<T, TLocalService, TEntity, TPrimaryKey>(TLocalService localService)
    : SyncServiceBase<T, int>,
        IIncrementSyncService<T>
    where TLocalService :
    IDbService<TEntity, TPrimaryKey>
    where TEntity :
    RemoteSyncEntityBase
{
    /// <summary>
    /// 查询最新版本的URL地址
    /// </summary>
    protected abstract string QueryLatestVersionUrl { get; }

    protected override async Task<IResultModel<int>> GetConditionAsync()
    {
        var queryResult = await HttpClient.GetAsync(QueryLatestVersionUrl);
        if (queryResult.IsSuccessStatusCode)
        {
            var content = await queryResult.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(content))
                return ResultModelFactory.Success<int>(0);

            return ResultModelFactory.Success<int>(int.Parse(content));
        }
        else
        {
            return ResultModelFactory.Error<int>(queryResult.ReasonPhrase ?? string.Empty);
        }
    }

    public Task<IResultModel<int>> QueryLatestVersionAsync()
    {
        return GetConditionAsync();
    }

    protected override async Task<IResultModel<bool>> WriteToLocalDbAsync(T data)
    {
        await localService.DeleteRangeAsync(x => x.Email == User.Email);
        return await UpdateLocalDbAsync(data);
    }

    protected abstract Task<IResultModel<bool>> UpdateLocalDbAsync(T data);

    public override async Task<IResultModel<bool>> SyncAsync()
    {
        var remoteVersionResult = await GetConditionAsync();
        if (remoteVersionResult is not { Ok: true, Data: var remoteVersion })
        {
            Log.Error("从{0}地址获得远程版本号错误,错误消息:{1}", QueryLatestVersionUrl, remoteVersionResult.ErrorMsg);
            return ResultModelFactory.Error<bool>(remoteVersionResult.ErrorMsg);
        }

        var currentData = (await localService.QueryAsync(x => User.Email == x.Email)).ToArray();
        var localVersion = 0;
        if (currentData.Any()) localVersion = currentData.MaxBy(x => x.Version)?.Version ?? 0;

        //当本地版本小于远程版本，从远程进行更新
        if (localVersion < remoteVersion)
        {
            Logger.LogInformation("{0}:当前版本({1})小于远程版本({2}),从远程下载新版本", GetType().Name, localVersion, remoteVersion);
            return await SyncDataAsync(remoteVersion);
        }
        else if (localVersion > remoteVersion || (localVersion == 0 && remoteVersion == 0))
        {
            Logger.LogInformation("{0}:当前版本({1})大于远程版本({2}),从本地上传到服务器", GetType().Name, localVersion, remoteVersion);

            var uploadVersion = localVersion + 1;
            var data = await localService.QueryAsync(x => x.Version == localVersion && x.Email == User.Email);

            foreach (var item in data)
            {
                item.Version = uploadVersion;
                item.LastUpdateTime = DateTime.Now;
            }

            await localService.UpdateRangeAsync(data);
            await UploadNewVersionToRemoteAsync(data);
            return ResultModelFactory.Success<bool>(true);
            ;
        }
        else
        {
            Logger.LogInformation("{0}:当前版本({1})等于远程版本({2})", GetType().Name, localVersion, remoteVersion);
            return ResultModelFactory.Success<bool>(true);
            ;
        }
    }

    protected abstract Task UploadNewVersionToRemoteAsync(IEnumerable<TEntity> data);
}