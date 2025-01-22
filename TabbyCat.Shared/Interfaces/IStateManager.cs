namespace TabbyCat.Shared.Interfaces;

public interface IStateManager
{
    Action? StateChanged { get; set; }

    public void SetState<T>(string key, T? value);

    public T? GetState<T>(string key);

    public bool ExistKey(string key);

    public void Cleaer();
}