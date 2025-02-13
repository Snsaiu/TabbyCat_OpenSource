namespace TabbyCat.App.Interfaces
{
    /// <summary>
    /// 打开程序
    /// </summary>
    public interface IAppLauncher
    {
        bool LaunchApp(string identifier);
    }
}