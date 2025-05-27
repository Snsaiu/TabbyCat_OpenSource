namespace TabbyCat.Repository.Entities;

/// <summary>
/// 支持远程同步的模型基类
/// </summary>
public abstract class RemoteSyncEntityBase : UserBaseEntity
{
    /// <summary>
    /// 版本号
    /// </summary>
    public int Version { get; set; }
    
    /// <summary>
    /// 最后一次更新时间
    /// </summary>
    public DateTime LastUpdateTime { get; set; }
    
    /// <summary>
    /// 是否已经删除
    /// </summary>
    public bool IsDeleted { get; set; }
}