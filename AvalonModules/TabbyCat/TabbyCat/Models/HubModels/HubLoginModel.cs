namespace TabbyCat.Models.HubModels;

public class HubLoginModel
{
    public string DeviceType { get; }
    public DateTime LoginTime { get; }
    public string Position { get; set; }

    [JsonConstructor]
    public HubLoginModel(string deviceType, DateTime loginTime)
    {
        DeviceType = deviceType;
        LoginTime = loginTime;
    }

    // 无参构造可选：如果只在本地创建时需要
    public HubLoginModel()
    {
        DeviceType = GetDeviceType();
        LoginTime = DateTime.Now;
    }

    private string GetDeviceType()
    {
        if (OperatingSystem.IsWindows())
            return "Windows";
        if (OperatingSystem.IsMacOS())
            return "MacOS";
        if (OperatingSystem.IsLinux())
            return "Linux";
        if (OperatingSystem.IsAndroid())
            return "Android";
        if (OperatingSystem.IsIOS())
            return "iOS";
        return "Unknown";
    }
}