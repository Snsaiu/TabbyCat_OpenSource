using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.Extensions;
using TabbyCat.Models;
using TabbyCat.Repository;
using TabbyCat.Repository.Entities;
using TuDog.Bootstrap;
using TuDog.Extensions;
using YouYan.Hamster.ResultModels;

namespace TabbyCat.IServices;

/// <summary>
/// 同步服务基类
/// </summary>
/// <param name="httpClientFactory">http客户端工厂<see cref="IHttpClientFactory"/></param>
/// <param name="user"><see cref="IUser"/>接口</param>
/// <typeparam name="T">服务器同步到客户端的数据类型，通常<see cref="IEnumerable{T}"/>类型</typeparam>
/// <typeparam name="TCondition">查询条件的类型，如果是增量同步，通常是<see cref="int"/></typeparam>
/// <typeparam name="TLocalService"></typeparam>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TKeyType"></typeparam>
public abstract class SyncServiceBase<T, TCondition> : ISyncService<T>
{
    protected IUser User => TuDogApplication.ServiceProvider.GetRequiredService<IUser>();

    protected HttpClient HttpClient { get; } = TuDogApplication.ServiceProvider.GetRequiredService<IHttpClientFactory>()
        .CreateClient(ConstParameter.Auth);

    /// <summary>
    /// 将本地数据推送到远程的URL地址
    /// </summary>
    protected abstract string UploadUrl { get; }

    /// <summary>
    /// 将远程数据同步到本地的URL地址
    /// </summary>
    protected abstract string SyncUrl { get; }

    protected async Task<IResultModel<bool>> SyncDataAsync(TCondition version)
    {
        var downloadSettingDto = new DownloadSettingDto<TCondition> { Email = User.Email, Version = version };

        var json = JsonConvert.SerializeObject(downloadSettingDto);
        var content = new StringContent(json);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await HttpClient.PostAsync(SyncUrl, content);
        response.EnsureSuccessStatusCode();
        var result = !response.IsSuccessStatusCode
            ? ResultModelFactory.Error<T>(response.ReasonPhrase)
            : ResultModelFactory.Success<T>(
                JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync()));
        if (result is { Ok: true, Data: var data })
        {
            return await WriteToLocalDbAsync(result.Data);
        }
        else
        {
            Logger.LogError("同步发生错误:{0}", result.ErrorMsg);
            return ResultModelFactory.Error<bool>(result.ErrorMsg);
        }
    }

    protected abstract Task<IResultModel<bool>> WriteToLocalDbAsync(T data);

    /// <summary>
    /// 获得与服务器的比对条件数据
    /// </summary>
    /// <returns></returns>
    protected abstract Task<IResultModel<TCondition>> GetConditionAsync();

    public virtual async Task<IResultModel<bool>> SyncAsync()
    {
        var conditionResult = await GetConditionAsync();
        if (conditionResult is not { Ok: true, Data: var data })
            return ResultModelFactory.Error<bool>(conditionResult.ErrorMsg);
        else
            return await SyncDataAsync(data);
    }

    public async Task UploadRemoteAsync(T data)
    {
        var setting = new UploadDto<T>
        {
            Email = User.Email,
            Data = data
        };
        await HttpClient.PostRequestAsync(UploadUrl, setting);
        await OnUpdatedAsync(data);
    }

    protected virtual Task OnUpdatedAsync(T data)
    {
        return Task.CompletedTask;
    }
}