using TabbyCat.Repository.Entities.AiMediaEntities;
using TabbyCat.Shared.Enums;

namespace TabbyCat.IServices;

public interface IAiMediaRunningStateManager
{
    public Task<bool> AddTaskAsync(AiMediaRunningStateEntity? entity);

    public Task StopWatchAsync();

    public Task StartWatchAsync();

    public Task ClearTasksAsync();

    public Func<string, AiMediaWorkType, Task> OnSuccess { get; set; }
    public Func<string, Task>? OnFailure { get; set; }

    public Func<int, Task>? OnBackgroundTaskCount { get; set; }
}