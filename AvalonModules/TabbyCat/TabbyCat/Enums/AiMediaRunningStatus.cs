namespace TabbyCat.Enums;

public enum AiMediaRunningStatus
{
    /// <summary>
    /// 任务排队中
    /// </summary>
    Pending,
    
    /// <summary>
    /// 任务处理中
    /// </summary>
    Running,
    
    /// <summary>
    /// 任务挂起
    /// </summary>
    Suspended,
    
    /// <summary>
    /// 任务执行成功
    /// </summary>
    Success,
    
    /// <summary>
    /// 任务执行失败
    /// </summary>
    Failed,
    
    /// <summary>
    /// 任务不存在或状态未知
    /// </summary>
    Unknown
}