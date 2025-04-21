using System.Diagnostics.CodeAnalysis;

namespace TuDog.Interfaces.Navigations.Impl;

public class NavigationParameter : INavigationParameter
{

    private Dictionary<string, object> _params = [];

    public void Add(string key, object value)
    {
        foreach (string item in _params.Keys)
        {
            if (item == key)
                throw new ArgumentException($"key值 {key}已经存在！但是{key}不能重复!");

        }
        _params[key] = value;
    }

    public object Get(string key)
    {
        return _params.TryGetValue(key, out object value) ? value : throw new NullReferenceException($"{key}值找不到对应的value");
    }

    public T Get<T>(string key)
    {
        return _params.TryGetValue(key, out object value)
            ? value is T x ? x : throw new Exception($"参数{key}的value 无法转换为{typeof(T)}")
            : throw new NullReferenceException($"{key}值找不到对应的value");
    }

    public bool HasKey(string key)
    {
        return _params.Keys.Contains(key);
    }

    public bool TryAndGet<T>(string key, [MaybeNullWhen(false)]out T value)
    {
        var result = _params.TryGetValue(key, out object? v);
        if(result)
        {
            if (v is T t)
            {
                value = t;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
        else
        {
            value = default;
            return result;
        }
       
    }
}