using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading.Tasks;

using TuDog.Extensions;
using TuDog.IocAttribute;

using YouYan.Rabbit.Extensions;
using YouYan.Rabbit.IServices;
using YouYan.Rabbit.IServices.LocalConfigs;
using YouYan.Rabbit.Models;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace YouYan.Rabbit.ViewModels;

[Register]
public sealed partial class AppListViewModel(IAppStateService appStateService,
    HttpClient httpClient,
    IAppInstallPathService appInstallPathService,
    ICacheFolderService cacheFolderService) : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<AppListItemModel> availableApps = [];

    [ObservableProperty] private ObservableCollection<AppListItemModel> installedApps = [];

    private Thread loadReleasesThread;

    protected override async Task OnLoaded()
    {
        await LoadInstalledAppsAsync();
        loadReleasesThread = new(LoopCheckVersion);
        loadReleasesThread.IsBackground = true;
        loadReleasesThread.Start();
    }

    protected override Task OnUnLoaded()
    {
        loadReleasesThread.Interrupt();
        return base.OnUnLoaded();
    }

    private async Task LoadInstalledAppsAsync()
    {
        var apps = Enum.GetValues<AppName>();
        foreach (var app in apps)
        {
            if (await appStateService.QueryAppExistsAsync(app))
            {
                var startFolder = await appStateService.QueryAppLocationAsync(app);
                var version = await appStateService.QueryAppInstalledVersionAsync(app);
                if (version is not null)
                    InstalledApps.Add(new() { AppName = app, InstallLocation = startFolder, Version = version });
            }
            else
                AvailableApps.Add(new() { AppName = app });
        }
    }


    private void LoopCheckVersion()
    {
        while (true)
        {
            var apps = Enum.GetValues<AppName>();
            foreach (var item in apps)
            {
                Thread.Sleep(TimeSpan.FromSeconds(20));
                var result = appStateService.QueryLatestReleaseAsync(item).GetAwaiter().GetResult();
                if (result is null)
                    continue;
                var installed = InstalledApps.FirstOrDefault(x => x.AppName == item);
                if (installed is not null)
                {
                    if (installed.Status is AppInstallStatus.Downloading or AppInstallStatus.Installing)
                        continue;

                    if (installed.LatestVersion != result.Version)
                    {
                        installed.LatestVersion = result.Version;
                        installed.Description = result.Description;
                    }
                }
                else
                {
                    var ava = AvailableApps.FirstOrDefault(x => x.AppName == item);
                    if (ava is not null)
                    {
                        if (ava.Status is AppInstallStatus.Downloading or AppInstallStatus.Installing)
                            continue;
                        if (ava.LatestVersion != result.Version)
                        {
                            ava.LatestVersion = result.Version;
                            ava.Description = result.Description;
                        }
                    }
                }
            }
        }
    }

    [RelayCommand]
    private async Task InstallApp(AppListItemModel selected)
    {
        var url = Properties.Resources.DownloadUrlBase +
                  $"/api/app/software-base/down-load?appName={selected.AppName.ToString()}";

        var cacheFolder = cacheFolderService.Get();
        var downloadAppName = Path.Combine(cacheFolder, selected.AppName.ToString() + ".zip");
        if (File.Exists(downloadAppName))
            File.Delete(downloadAppName);

        var unzipFolder = Path.Combine(cacheFolder, selected.AppName.ToString());

        var installPath = string.Empty;
        if (OperatingSystem.IsWindows())
        {
            var youyanPath = appInstallPathService.GetAppInstallPath();

            if (!Directory.Exists(youyanPath)) Directory.CreateDirectory(youyanPath);
            installPath = Path.Combine(youyanPath, selected.AppName.ToString());
            if (!Directory.Exists(installPath)) Directory.CreateDirectory(installPath);

            SetPermissions(installPath);

            selected.Status = AppInstallStatus.Waiting;
            await httpClient.DownloadFileAsync(url, downloadAppName, x =>
            {
                selected.Status = AppInstallStatus.Downloading;
                selected.DownloadProgress = x;
            });
            if (!File.Exists(downloadAppName))
            {
                selected.Status = AppInstallStatus.Available;
                await DialogServer.ShowMessageDialogAsync("下载失败");
                return;
            }

            selected.Status = AppInstallStatus.Installing;
            using (var zipfile = ZipFile.OpenRead(downloadAppName))
            {
                if (Directory.Exists(unzipFolder))
                    Directory.Delete(unzipFolder, true);
                Directory.CreateDirectory(unzipFolder);

                zipfile.ExtractToDirectory(unzipFolder);
                var directoryInfo = new DirectoryInfo(unzipFolder);
                var son = directoryInfo.GetDirectories();
                if (son.Length <= 0)
                {
                    selected.Status = AppInstallStatus.Failed;
                    return;
                }

                CopyFilesRecursively(son.First().FullName, installPath);
            }

            selected.Status = AppInstallStatus.Installed;
            await appStateService.WriteAppVersionAsync(selected.AppName, selected.Version);
            // 清空缓存

            if (File.Exists(downloadAppName))
                File.Delete(downloadAppName);
            if (Directory.Exists(unzipFolder))
                Directory.Delete(unzipFolder, true);

            await Task.Delay(TimeSpan.FromSeconds(1));
            InstalledApps.Add(selected);
            AvailableApps.Remove(selected);
        }
        else if (OperatingSystem.IsLinux())
        {
        }
        else if (OperatingSystem.IsMacOS())
        {
        }
        else
        {
            throw new NotImplementedException();
        }

        //

    }

    #region 工具

    private static void CopyFilesRecursively(string sourcePath, string destinationPath)
    {
        // 确保目标目录存在
        Directory.CreateDirectory(destinationPath);

        // 拷贝当前目录中的所有文件
        foreach (var file in Directory.GetFiles(sourcePath))
        {
            var fileName = Path.GetFileName(file);
            var destFilePath = Path.Combine(destinationPath, fileName);

            // 覆盖已有文件
            File.Copy(file, destFilePath, true);
        }

        // 递归拷贝子目录中的文件
        foreach (var subDir in Directory.GetDirectories(sourcePath))
        {
            var dirName = Path.GetFileName(subDir);
            var destSubDir = Path.Combine(destinationPath, dirName);

            CopyFilesRecursively(subDir, destSubDir);
        }
    }

    private static void SetPermissions(string path)
    {
        // icacls 命令：将 "Users" 组授予完全控制权限
        var psi = new ProcessStartInfo
        {
            FileName = "icacls",
            Arguments = $"\"{path}\" /grant Users:(F)",
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = Process.Start(psi))
        {
            process?.WaitForExit();
        }
    }

    #endregion

}