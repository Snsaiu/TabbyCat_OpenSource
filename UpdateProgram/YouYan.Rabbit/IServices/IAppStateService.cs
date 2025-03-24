using System.Threading.Tasks;
using YouYan.Rabbit.Extensions;
using YouYan.Rabbit.Models;

namespace YouYan.Rabbit.IServices;

public interface IAppStateService
{
    /// <summary>
    /// 检查是否存在
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    Task<bool> QueryAppExistsAsync(AppName app);

    Task<string?> QueryAppInstalledVersionAsync(AppName app);

    Task<AppReleaseModel?> QueryLatestReleaseAsync(AppName app);

    Task<AppReleaseModel?> QueryLatestReleaseAsync(string app);

    Task<string> QueryAppLocationAsync(AppName app);

    Task WriteAppVersionAsync(AppName app, string version);

    Task LaunchAppAsync(AppName app, string? customAppName = null, bool single = true, string[]? args = null);

    Task<bool> AppIsRunningAsync(AppName app, string? customAppName = null);
    Task<bool> UninstallAppAsync(AppName app);

    Task<string?> QueryAppInstalledVersionAsync(string name);


}