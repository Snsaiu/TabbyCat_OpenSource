using System.Collections.Concurrent;

using TabbyCat.Shared.Interfaces;

namespace TabbyCat.App.Interfaces.Impls;

public class StateManager : IStateManager
{
    private readonly ConcurrentDictionary<string, object?> stateDictionary = [];

    public Action? StateChanged { get; set; }

    public void SetState<T>(string key, T? value)
    {
        stateDictionary[key] = value;
        StateChanged?.Invoke();
    }

    public T? GetState<T>(string key)
    {
        return !stateDictionary.TryGetValue(key, out var value)
            ? throw new ArgumentNullException($"字典中没有Key为{key}的值")
            : (T?)value;
    }

    public bool ExistKey(string key)
    {
        return stateDictionary.ContainsKey(key);
    }

    public void Cleaer()
    {
        stateDictionary.Clear();
    }
}