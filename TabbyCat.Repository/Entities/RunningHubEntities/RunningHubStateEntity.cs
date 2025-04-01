using TabbyCat.Shared.Enums;

namespace TabbyCat.Repository.Entities.RunningHubEntities;

public class RunningHubStateEntity:UserBaseEntity
{
    public string TaskId  { get; set; }

    public string ClientId { get; set; }

    public string ApiKey { get; set; }

    public TaskState TaskStatus { get; set; }

    public RunningHubWorkType RunningHubWorkType { get; set; }

    public static RunningHubStateEntity Create(string taskId, string clientId, string apiKey,
        RunningHubWorkType runningHubWorkType)
    {
        var runningHubStateEntity = new RunningHubStateEntity()
        {
            TaskId = taskId, ClientId = clientId, TaskStatus = TaskState.Running, ApiKey = apiKey,
            RunningHubWorkType = runningHubWorkType
        };
        return runningHubStateEntity;
    }
}