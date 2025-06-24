using TabbyCat.Models.HubModels;
using TabbyCat.Repository.Entities;

namespace TabbyCat.IServices;

/// <summary>
/// 使用SignalR增量数据同步管理器
/// </summary>
public interface
    IIncrementHubSyncManager<TReceiveModel, in THubTransferModelBase, TSendModel> : IHubSyncManager<TReceiveModel,
    THubTransferModelBase, TSendModel>
    where TReceiveModel : RemoteSyncEntityBase
    where THubTransferModelBase : HubTransferModelBase<IEnumerable<TSendModel>>;