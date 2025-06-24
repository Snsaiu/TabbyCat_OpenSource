using Microsoft.Extensions.DependencyInjection;
using TabbyCat.Models.HubModels;
using TabbyCat.Repository;
using TabbyCat.Repository.Entities;
using TuDog.Bootstrap;

namespace TabbyCat.IServices;

/// <summary>
/// hub同步管理器基类
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TLocalService"></typeparam>
/// <typeparam name="TLocalPrimaryKeyType"></typeparam>
/// <typeparam name="THubTransferModel"></typeparam>
public abstract class
    HubSyncManager<TEntity, TLocalService, TLocalPrimaryKeyType,
        THubTransferModel> : IHubSyncManager<TEntity, THubTransferModel, TEntity>
    where TLocalService : IDbService<TEntity, TLocalPrimaryKeyType>
    where TEntity : UserBaseEntity
    where THubTransferModel : HubTransferModelBase<IEnumerable<TEntity>>
{
    protected IUser User => TuDogApplication.ServiceProvider.GetRequiredService<IUser>();

    protected IHubService HubService { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IHubService>();


    protected TLocalService LocalService { get; }

    public HubSyncManager(TLocalService localService)
    {
        LocalService = localService;
    }

    /// <summary>
    /// 回调本地方法
    /// </summary>
    public abstract string HubNotifyMethodName { get; }

    /// <summary>
    /// 调用hub服务端的方法
    /// </summary>
    public abstract string SendHubMethodName { get; }

    private Func<THubTransferModel, Task> _callback;

    public Task InitializeAsync()
    {
        _callback += UpdateCallBackAsync;
        HubService.Register(HubNotifyMethodName, _callback);
        return Task.CompletedTask;
    }

    public abstract Func<IEnumerable<TEntity>, Task>? UpdatedCallBack { get; set; }

    public Task SyncAsync(THubTransferModel sendModel)
    {
        return HubService.SendMessageAsync(SendHubMethodName, sendModel);
    }

    /// <summary>
    /// hub回调实现
    /// </summary>
    /// <param name="model">数据模型<see cref="HubTransferModelBase{T}"/></param>
    private async Task UpdateCallBackAsync(THubTransferModel model)
    {
        await UpdateAsync(model);
        UpdatedCallBack?.Invoke(model.Data);
    }

    /// <summary>
    /// 更新数据库，直接插入
    /// </summary>
    /// <param name="model"> <see cref="HubTransferModelBase{T}"/></param>
    protected virtual async Task UpdateAsync(THubTransferModel model)
    {
        await LocalService.AddRangeAsync(model.Data);
    }
}