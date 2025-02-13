using System.Text.Json;

public static class CloneHelper
{
    public static T? DeepClone<T>(T obj)
    {
        if (obj == null)
            return default(T);
        var json = JsonSerializer.Serialize(obj);
        return JsonSerializer.Deserialize<T>(json) ?? throw new NullReferenceException("Deserialized object is null");
    }
}
