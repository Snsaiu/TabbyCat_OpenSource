namespace TabbyCat.Repository.Entities.RunningHubEntities;

public class RunningHubResultEntity : UserBaseEntity
{
    public string TaskId { get; set; }
    public string SavePath { get; set; }
    public string FileType { get; set; }
}