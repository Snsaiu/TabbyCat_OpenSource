using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;

namespace TuDog.Interfaces.PreferenceServices.Impl;

public sealed class JsonPreferenceService : IPreferenceService
{
    private const string fileName = "preferences.json";
    private readonly string fullFilePath;

    public JsonPreferenceService()
    {
        if (OperatingSystem.IsAndroid())
            // 返回应用的配置目录
            fullFilePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
        else
            fullFilePath = Path.Join(Path.GetDirectoryName(Environment.ProcessPath), fileName);
        if (!File.Exists(fullFilePath))
            File.WriteAllText(fullFilePath, "{}");


    }

    public void Set<T>(string key, T value)
    {
        var json = File.ReadAllText(fullFilePath);
        var config = JObject.Parse(json);
        config[key] = JToken.FromObject(value);
        File.WriteAllText(fullFilePath, config.ToString(Formatting.Indented));
    }

    public T Get<T>(string key, T defaultValue)
    {
        var json = File.ReadAllText(fullFilePath);
        var config = JObject.Parse(json);
        if (!config.ContainsKey(key))
        {
            Set(key, defaultValue);
            return defaultValue;
        }

        var obj = config[key].ToObject<T>();
        return obj;
    }
}