using TabbyCat.Models.HubModels;

namespace TabbyCat.IServices;

/// <summary>
/// Hub同步管理器
/// </summary>
/// <typeparam name="TReceiveModel"></typeparam>
/// <typeparam name="THubTransferModelBase"></typeparam>
/// <typeparam name="TSendModel"></typeparam>
public interface IHubSyncManager<TReceiveModel, in THubTransferModelBase, TSendModel>
    where THubTransferModelBase : HubTransferModelBase<IEnumerable<TSendModel>>
{
    /// <summary>
    /// 初始化
    /// </summary>
    /// <returns></returns>
    Task InitializeAsync();

    /// <summary>
    /// 更新完成回调
    /// </summary>
    Func<IEnumerable<TReceiveModel>, Task>? UpdatedCallBack { get; set; }

    /// <summary>
    /// 发送信息到hub请求更新其他客户端
    /// </summary>
    /// <param name="sendModel"></param>
    /// <returns></returns>
    Task SyncAsync(THubTransferModelBase sendModel);
}