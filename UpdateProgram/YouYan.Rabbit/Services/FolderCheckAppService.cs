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
using System.Runtime.InteropServices;
using TuDog.Extensions;
using YouYan.Rabbit.Models;

namespace YouYan.Rabbit.Services;

[Register<IAppStateService>]
public sealed class FolderCheckAppService(IAppInstallPathService appInstallPathService, HttpClient httpClient)
    : IAppStateService
{
    public Task<bool> QueryAppExistsAsync(AppName app)
    {
        if (OperatingSystem.IsWindows() || OperatingSystem.IsMacOS())
        {
            var folder = Path.Combine(appInstallPathService.GetAppInstallPath(), app.ToString());
            if (!Directory.Exists(folder))
                return Task.FromResult(false);
            var v = Directory.GetFiles(folder, "v");
            return Task.FromResult(v.Length > 0);
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


    public async Task<string?> QueryAppInstalledVersionAsync(string name)
    {
        if (OperatingSystem.IsWindows() || OperatingSystem.IsMacOS())
        {
            var folder = name == "Rabbit"
                ? Path.Combine(appInstallPathService.GetRabbitInstallPath(), name)
                : Path.Combine(appInstallPathService.GetAppInstallPath(), name);
            if (!Directory.Exists(folder))
                return null;
            var versionFile = Directory.GetFiles(folder, "v");
            if (versionFile.Length > 0) return await File.ReadAllTextAsync(versionFile[0]);

            return null;
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


    public Task<string?> QueryAppInstalledVersionAsync(AppName app)
    {
        return QueryAppInstalledVersionAsync(app.ToString());
    }


    public Task<AppReleaseModel?> QueryLatestReleaseAsync(AppName app)
    {
        return QueryLatestReleaseAsync(app.ToString());
    }

    public Task<AppReleaseModel?> QueryLatestReleaseAsync(string app)
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

        queryReleaseModel.AppName = app;

        return httpClient.PostRequestAsync<QueryReleaseModel, AppReleaseModel>(url, queryReleaseModel);
    }

    public Task<string> QueryAppLocationAsync(AppName app)
    {
        if (OperatingSystem.IsWindows() || OperatingSystem.IsMacOS())
        {
            return Task.FromResult(Path.Combine(appInstallPathService.GetAppInstallPath(), app.ToString()));
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
        if (OperatingSystem.IsWindows() || OperatingSystem.IsMacOS())
        {
            var folder = Path.Combine(appInstallPathService.GetAppInstallPath(), app.ToString());
            if (!Directory.Exists(folder))
                throw new InvalidOperationException($"文件夹{folder}不存在");
            await File.WriteAllTextAsync(Path.Combine(folder, "v"), version);
        }
        else if (OperatingSystem.IsLinux())
        {
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    // 导入 user32.dll，调用 SetForegroundWindow API
    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hwnd);

    private static Process? FindProcessByName(string processName)
    {
        // 查找指定名称的进程
        foreach (var process in Process.GetProcessesByName(processName))
            return process;

        return null; // 如果没有找到对应的进程
    }

    public Task LaunchAppAsync(AppName app, string? customAppName = null, bool single = true, string[]? args = null)
    {
        if (OperatingSystem.IsWindows())
        {
            var folder = Path.Combine(appInstallPathService.GetAppInstallPath(), app.ToString());
            if (!Directory.Exists(folder))
                throw new InvalidOperationException($"文件夹{folder}不存在，无法启动程序");
            var appName = customAppName ?? app.ToString();
            var fullPath = Path.Combine(folder, appName + ".exe");

            if (Process.GetProcessesByName(appName).Length > 0 && single) return Task.CompletedTask;

            Process.Start(new ProcessStartInfo
            {
                FileName = fullPath, // 指定可执行文件路径
                UseShellExecute = true // 确保以默认应用打开
            });

            return Task.CompletedTask;
        }
        else if (OperatingSystem.IsMacOS())
        {
            Process.Start("open", $"-a {app.ToString()}");
        }
        else if (OperatingSystem.IsLinux())
        {
        }
        else
        {
            throw new NotImplementedException();
        }

        return Task.CompletedTask;
    }

    public Task<bool> AppIsRunningAsync(AppName app, string? customAppName = null)
    {
        if (OperatingSystem.IsWindows())
        {
            var folder = Path.Combine(appInstallPathService.GetAppInstallPath(), app.ToString());
            if (!Directory.Exists(folder))
                throw new InvalidOperationException($"文件夹{folder}不存在");
            var appName = customAppName ?? app.ToString();

            return Task.FromResult(Process.GetProcessesByName(appName).Length > 0);
        }
        else if (OperatingSystem.IsMacOS())
        {
            var appName = app.ToString(); // 应用名
            var command = $"ps aux | grep '{appName}.app' | grep -v 'grep'";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{command}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return Task.FromResult(!string.IsNullOrWhiteSpace(output));
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

    /// <summary>
    /// 调用这个方法要先调用AppIsRunningAsync 检查是否在运行
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public async Task<bool> UninstallAppAsync(AppName app)
    {
        var location = await QueryAppLocationAsync(app);

        try
        {
            Directory.Delete(location, true);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}