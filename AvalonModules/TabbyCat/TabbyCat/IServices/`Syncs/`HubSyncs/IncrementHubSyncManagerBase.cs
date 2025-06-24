using Microsoft.Extensions.DependencyInjection;
using TabbyCat.Models.HubModels;
using TabbyCat.Repository;
using TabbyCat.Repository.Entities;
using TuDog.Bootstrap;

namespace TabbyCat.IServices;

/// <summary>
/// 增量数据同步管理器基类
/// </summary>
public abstract class
    IncrementHubSyncManagerBase<TEntity, TLocalService, TLocalPrimaryKeyType,
        THubTransferModel>(TLocalService localService) : HubSyncManager<TEntity, TLocalService, TLocalPrimaryKeyType,
    THubTransferModel>(localService), IIncrementHubSyncManager<TEntity, THubTransferModel, TEntity>
    where TLocalService :
    IDbService<TEntity, TLocalPrimaryKeyType>
    where TEntity :
    RemoteSyncEntityBase
    where THubTransferModel :
    HubTransferModelBase<IEnumerable<TEntity>>
{
    /// <summary>
    /// 更新数据库，先清空当前表然后重新插入最新的版本
    /// </summary>
    /// <param name="model"> <see cref="HubTransferModelBase{T}"/></param>
    protected override async Task UpdateAsync(THubTransferModel model)
    {
        await LocalService.DeleteRangeAsync(x => x.Email == User.Email);
        await LocalService.AddRangeAsync(model.Data);
    }
}