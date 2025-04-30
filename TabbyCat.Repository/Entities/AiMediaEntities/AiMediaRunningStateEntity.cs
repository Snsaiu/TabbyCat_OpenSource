using TabbyCat.Shared.Enums;

namespace TabbyCat.Repository.Entities.AiMediaEntities;

public class AiMediaRunningStateEntity:UserBaseEntity
{
    public string TaskId { get; set; } = string.Empty;

    public TaskState TaskStatus { get; set; }

    public string ApiKey { get; set; } = string.Empty;

    public AiMediaWorkType WorkType { get; set; }

    public string FileExtension { get; set; } = string.Empty;

    public static AiMediaRunningStateEntity Create(string taskId, string apiKey, AiMediaWorkType workType, string email,
        string extension)
    {
        var stateEntity = new AiMediaRunningStateEntity()
        {
            TaskId = taskId, TaskStatus = TaskState.Running,
            WorkType = workType,
            ApiKey = apiKey,
            Email = email,
            FileExtension = extension
        };
        return stateEntity;
    }
}