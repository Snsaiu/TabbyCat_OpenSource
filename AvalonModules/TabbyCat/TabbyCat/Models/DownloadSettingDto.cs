namespace TabbyCat.Models;

public class DownloadSettingDto<T>
{
    public string Email { get; set; } = string.Empty;

    public T Version { get; set; }
}