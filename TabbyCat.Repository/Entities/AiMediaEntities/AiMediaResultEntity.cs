using TabbyCat.Shared.Enums;

namespace TabbyCat.Repository.Entities.AiMediaEntities;

public class AiMediaResultEntity : UserBaseEntity
{
    public string TaskId { get; set; }
    public string SavePath { get; set; }
    public string FileType { get; set; }

    /// <summary>
    /// 缩略图，针对视频有效
    /// </summary>
    public string ThumbnailPath { get; set; }

    public AiMediaWorkType WorkType { get; set; }
}