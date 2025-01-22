namespace TabbyCat.Shared.Extensions
{
    public static class TaskExtensions
    {
        public static async void WaitTask(this Task task, Action? onSuccess, Action<Exception>? onError)
        {
            try
            {
                await task;
                onSuccess?.Invoke();
            }
            catch (Exception e)
            {
                onError?.Invoke(e);
            }
        }
    }
}