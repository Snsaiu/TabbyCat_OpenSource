using System;
using System.Threading.Tasks;
using Microsoft.Win32;
using TuDog.IocAttribute;
using YouYan.Rabbit.Extensions;
using YouYan.Rabbit.IServices;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using TuDog.Extensions;
using YouYan.Rabbit.Models;

namespace YouYan.Rabbit.Services;

[Register<IAppStateService>]
public sealed class FolderCheckAppService(IAppInstallPathService appInstallPathService, HttpClient httpClient)
    : IAppStateService
{
    public Task<bool> QueryAppExistsAsync(AppName app)
    {
        if (OperatingSystem.IsWindows())
        {
            var folder = Path.Combine(appInstallPathService.GetAppInstallPath(), app.ToString());
            if (!Directory.Exists(folder))
                return Task.FromResult(false);
            var v = Directory.GetFiles(folder, "v");
            return Task.FromResult(v.Length > 0);
        }
        else if (OperatingSystem.IsMacOS())
        {
        }
        else if (OperatingSystem.IsLinux())
        {
        }
        else
        {
            throw new NotImplementedException();
        }

        return Task.FromResult(false);
    }

    public async Task<string?> QueryAppInstalledVersionAsync(AppName app)
    {
        if (OperatingSystem.IsWindows())
        {
            var folder = Path.Combine(appInstallPathService.GetAppInstallPath(), app.ToString());
            if (!Directory.Exists(folder))
                return null;
            var versionFile = Directory.GetFiles(folder, "v");
            if (versionFile.Length > 0) return await File.ReadAllTextAsync(versionFile[0]);

            return null;
        }
        else if (OperatingSystem.IsMacOS())
        {
        }
        else if (OperatingSystem.IsLinux())
        {
        }
        else
        {
            throw new NotImplementedException();
        }

        return null;
    }

    public Task<AppReleaseModel?> QueryLatestReleaseAsync(AppName app)
    {
        var url = Properties.Resources.DownloadUrlBase + "/api/app/software-base/query-release";

        var queryReleaseModel = new QueryReleaseModel();

        if (OperatingSystem.IsWindows())
            queryReleaseModel.OsType = AppOsType.Windows;
        else if (OperatingSystem.IsMacOS())
            queryReleaseModel.OsType = AppOsType.MacOs;
        else if (OperatingSystem.IsLinux())
            queryReleaseModel.OsType = AppOsType.Ubuntu;
        else
            throw new NotImplementedException();

        queryReleaseModel.AppName = app.ToString();

        return httpClient.PostRequestAsync<QueryReleaseModel, AppReleaseModel>(url, queryReleaseModel);
    }

    public Task<string> QueryAppLocationAsync(AppName app)
    {
        if (OperatingSystem.IsWindows())
        {
            return Task.FromResult(Path.Combine(appInstallPathService.GetAppInstallPath(), app.ToString()));
        }
        else if (OperatingSystem.IsMacOS())
        {
        }
        else if (OperatingSystem.IsLinux())
        {
        }
        else
        {
            throw new NotImplementedException();
        }

        throw new NotImplementedException();
    }

    public async Task WriteAppVersionAsync(AppName app, string version)
    {
        if (OperatingSystem.IsWindows())
        {
            var folder = Path.Combine(appInstallPathService.GetAppInstallPath(), app.ToString());
            if (!Directory.Exists(folder))
                throw new InvalidOperationException($"文件夹{folder}不存在");
            await File.WriteAllTextAsync(Path.Combine(folder, "v"), version);
        }
        else if (OperatingSystem.IsMacOS())
        {
        }
        else if (OperatingSystem.IsLinux())
        {
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public Task LaunchAppAsync(AppName app, string? customAppName = null, string[]? args = null)
    {
        if (OperatingSystem.IsWindows())
        {
            var folder = Path.Combine(appInstallPathService.GetAppInstallPath(), app.ToString());
            if (!Directory.Exists(folder))
                throw new InvalidOperationException($"文件夹{folder}不存在，无法启动程序");
            var appName = customAppName ?? app.ToString();
            var fullPath = Path.Combine(folder, appName + ".exe");

            Process.Start(new ProcessStartInfo
            {
                FileName = fullPath, // 指定可执行文件路径
                UseShellExecute = true // 确保以默认应用打开
            });
        }
        else if (OperatingSystem.IsMacOS())
        {
        }
        else if (OperatingSystem.IsLinux())
        {
        }
        else
        {
            throw new NotImplementedException();
        }

        throw new NotImplementedException();
    }
}