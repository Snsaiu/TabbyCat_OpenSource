namespace TuDog.Interfaces.Navigations;

public interface INavigationParameter
{
    void Add(string key, object value);
    object Get(string key);

    T Get<T>(string key);

    bool HasKey(string key);
    
    bool TryAndGet<T>(string key, out T value);
}