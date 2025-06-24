namespace TabbyCat.IServices;

public interface IHubService
{
    Task StartAsync();

    Task StopAsync();

    Task SendMessageAsync<T>(string methodName, T? data);
    void Register<T>(string methodName, Func<T, Task>? callback);
}