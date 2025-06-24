using TabbyCat.Enums;

namespace TabbyCat.Models.HubModels;

/// <summary>
/// hub数据传递基类
/// </summary>
public abstract class HubTransferModelBase<T>(T data, HubTransferType transferType)
{
    public HubTransferType TransferType { get; set; } = transferType;

    public T Data { get; set; } = data;
}