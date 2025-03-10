using TabbyCat.Repository.Entities.RunningHubEntities;

namespace TabbyCat.IServices;

public interface IRunningHubStateManager
{
    public Task<bool> AddTaskAsync(RunningHubStateEntity? entity);

    public Task StopWatchAsync();

    public Task StartWatchAsync();

    public Func<Guid, Task> OnSuccess { get; set; }
    public Func<Guid, Task> OnFailure { get; set; }
    
    public Func<int,Task> OnBackgroundTaskCount { get; set; }
}