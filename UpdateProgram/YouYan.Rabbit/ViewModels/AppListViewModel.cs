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
using TuDog.Interfaces;
using TuDog.Interfaces.RegionManagers;
using YouYan.Rabbit.Components.ViewModels;
using YouYan.Rabbit.Languages;
using YouYan.Rabbit.Services;

namespace YouYan.Rabbit.ViewModels;

[Register]
public sealed partial class AppListViewModel(
    IAppStateService appStateService,
    HttpClient httpClient,
    ISystemService systemService,
    IRegionManager regionManager,
    IAppInstallPathService appInstallPathService,
    ICacheFolderService cacheFolderService) : ViewModelBase, IKeep
{
    private readonly IRegionManager _regionManager = regionManager;
    [ObservableProperty] private ObservableCollection<AppListItemModel> availableApps = [];

    [ObservableProperty] private ObservableCollection<AppListItemModel> installedApps = [];

    private Thread loadReleasesThread;

    private Thread appsRunningStatusThread;

    private Thread rabbitCheckVersionThread;

    [ObservableProperty] private bool newVersionAvailable;

    private bool isLoaded = false;

    protected override async Task OnLoaded()
    {
        if (isLoaded)
            return;
        isLoaded = true;

        await LoadInstalledAppsAsync();
        loadReleasesThread = new Thread(LoopCheckVersion);
        loadReleasesThread.IsBackground = true;
        loadReleasesThread.Start();

        appsRunningStatusThread = new Thread(LoopCheckAppRunningStatus);
        appsRunningStatusThread.IsBackground = true;
        appsRunningStatusThread.Start();

        rabbitCheckVersionThread = new Thread(LoopCheckRabbitVersion);
        rabbitCheckVersionThread.IsBackground = true;
        rabbitCheckVersionThread.Start();
    }


    [RelayCommand]
    private Task OpenSettingDialog()
    {
        _regionManager.AddToRegion<SettingViewModel>("container");
        return Task.CompletedTask;
    }


    [RelayCommand]
    private void OpenRabbitHole()
    {
        var cacheFolder = cacheFolderService.Get();
        var unzipFolder = Path.Combine(cacheFolder, "RabbitHole");

        if (OperatingSystem.IsWindows())
        {
            var exe = Path.Combine(unzipFolder, "RabbitHole.exe");
            Process.Start(new ProcessStartInfo
            {
                FileName = exe, // 指定可执行文件路径
                UseShellExecute = true // 确保以默认应用打开
            });
        }
    }

    private void LoopCheckRabbitVersion()
    {
        var currentVersion = appStateService.QueryAppInstalledVersionAsync("Rabbit").GetAwaiter().GetResult();
        while (true)
        {
            var result = appStateService.QueryLatestReleaseAsync("Rabbit").GetAwaiter().GetResult();
            if (result is null)
            {
                Thread.Sleep(TimeSpan.FromMinutes(10));
                continue;
            }

            if (currentVersion != result.Version)
            {
                var url = Properties.Resources.DownloadUrlBase +
                          $"/api/app/software-base/down-load?appName=RabbitHole&osType={(int)systemService.OsType}";

                var cacheFolder = cacheFolderService.Get();
                var downloadAppName = Path.Combine(cacheFolder, "RabbitHole.zip");
                if (File.Exists(downloadAppName))
                    File.Delete(downloadAppName);

                var unzipFolder = Path.Combine(cacheFolder, "RabbitHole");

                var downloadResult = httpClient.DownloadFileAsync(url, downloadAppName,
                    x => { Debug.WriteLine("下载rabbit hole 进度:" + x); }, error =>
                    {
                        //todo 写入日志
                    }).GetAwaiter().GetResult();
                if (downloadResult is true)
                {
                    using (var zipfile = ZipFile.OpenRead(downloadAppName))
                    {
                        if (Directory.Exists(unzipFolder))
                            Directory.Delete(unzipFolder, true);
                        Directory.CreateDirectory(unzipFolder);

                        zipfile.ExtractToDirectory(unzipFolder);
                    }

                    NewVersionAvailable = true;
                    break;
                }
            }

            Thread.Sleep(TimeSpan.FromMinutes(10));
        }

        rabbitCheckVersionThread.Interrupt();
    }


    protected override Task OnUnLoaded()
    {
        loadReleasesThread.Interrupt();
        appsRunningStatusThread.Interrupt();
        return base.OnUnLoaded();
    }

    /// <summary>
    /// 程序是否可以安全退出
    /// </summary>
    /// <returns></returns>
    public bool CanExit()
    {
        foreach (var item in InstalledApps)
            if (item.Status is AppStatus.Downloading or AppStatus.Installing or AppStatus.Uninstalling)
                return false;

        foreach (var item in AvailableApps)
            if (item.Status is AppStatus.Downloading or AppStatus.Installing or AppStatus.Uninstalling
                or AppStatus.Waiting)
                return false;

        return true;
    }

    [RelayCommand]
    private async Task UninstallApp(AppListItemModel selected)
    {
        var deleteConfirm = await DialogServer.ShowConfirmDialogAsync(Language.AreYouUninstall);
        if (!deleteConfirm)
            return;

        if (await appStateService.AppIsRunningAsync(selected.AppName, selected.ExeName()))
        {
            await DialogServer.ShowMessageDialogAsync(string.Format(Language.IsRunningNotUninstall,
                LocalizationResourceManager.Instance[selected.AppName.ToString()]));
            return;
        }

        if (await appStateService.UninstallAppAsync(selected.AppName))
        {
            InstalledApps.Remove(selected);
            selected.Status = AppStatus.Available;
            AvailableApps.Add(selected);
            // todo :接下去要提示是否删除配置文件

            var folder = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                selected.AppName.ToString());

            if (Directory.Exists(folder))
                if (await DialogServer.ShowConfirmDialogAsync(Language.DoYouDeleteCacheFile, Language.Warning,
                        Language.Ok))
                    Directory.Delete(folder, true);

            await DialogServer.ShowMessageDialogAsync(Language.UninstallSuccessfully, Language.Message, Language.Ok);
        }
        else
        {
            await DialogServer.ShowMessageDialogAsync(Language.UninstallationFailed, Language.Warning, Language.Ok);
        }
    }


    [RelayCommand]
    private async Task ShowWhatsNew(AppListItemModel selected)
    {
        var model = new AppReleaseModel();
        model.Description = selected.Description;
        model.Version = selected.LatestVersion;

        await DialogServer.ShowDialogAsync<WhatNewViewModel, AppReleaseModel, bool>(Language.WhatsNew,
            Language.Ok,
            string.Empty,
            model);
    }

    [RelayCommand]
    private async Task Launch(AppListItemModel selected)
    {
        await appStateService.LaunchAppAsync(selected.AppName, $"{selected.AppName}.Desktop");
    }


    private async Task<bool> InstallAsync(AppListItemModel selected)
    {
        selected.Status = AppStatus.Waiting;

        var url = Properties.Resources.DownloadUrlBase +
                  $"/api/app/software-base/down-load?appName={selected.AppName.ToString()}&osType={(int)systemService.OsType}";

        var cacheFolder = cacheFolderService.Get();
        var downloadAppName = Path.Combine(cacheFolder, selected.AppName.ToString() + ".zip");
        if (File.Exists(downloadAppName))
            File.Delete(downloadAppName);

        var unzipFolder = Path.Combine(cacheFolder, selected.AppName.ToString());

        var installPath = string.Empty;

        var youyanPath = appInstallPathService.GetAppInstallPath();

        if (!Directory.Exists(youyanPath)) Directory.CreateDirectory(youyanPath);
        installPath = Path.Combine(youyanPath, selected.AppName.ToString());

        if (OperatingSystem.IsWindows()) SetPermissions(installPath);

        if (!Directory.Exists(installPath)) Directory.CreateDirectory(installPath);

        var downloadResult = await httpClient.DownloadFileAsync(url, downloadAppName, x =>
        {
            selected.Status = AppStatus.Downloading;
            selected.DownloadProgress = x;
        }, error =>
        {
            //todo 写入日志
        });
        if (!downloadResult || !File.Exists(downloadAppName))
        {
            selected.Status = AppStatus.Available;
            await DialogServer.ShowMessageDialogAsync(Language.DownloadFailed, Language.Warning, Language.Ok);
            return false;
        }

        selected.Status = AppStatus.Installing;

        if (OperatingSystem.IsWindows())
        {
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
                    selected.Status = AppStatus.Failed;
                    return false;
                }

                CopyFilesRecursively(son.First().FullName, installPath);
            }
        }
        else if (OperatingSystem.IsMacOS())
        {
            using (var zipfile = ZipFile.OpenRead(downloadAppName))
            {
                if (Directory.Exists(installPath))
                    Directory.Delete(installPath, true);

                zipfile.ExtractToDirectory(installPath, true);

                var appInstallFullPath = Path.Combine(installPath, $"{selected.AppName}.app");
                if (!MacOsHelper.RemoveQuarantine(appInstallFullPath))
                {
                    await DialogServer.ShowMessageDialogAsync(Language.AppFailPermission, Language.Warning,
                        Language.Ok);
                    return false;
                }
            }
        }
        else if (OperatingSystem.IsLinux())
        {
        }
        else
        {
            throw new NotImplementedException();
        }

        selected.Version = selected.LatestVersion;
        selected.Status = AppStatus.Installed;
        await appStateService.WriteAppVersionAsync(selected.AppName, selected.Version);

        selected.InstallLocation = await appStateService.QueryAppLocationAsync(selected.AppName);

        // 清空缓存
        if (File.Exists(downloadAppName))
            File.Delete(downloadAppName);
        if (Directory.Exists(unzipFolder))
            Directory.Delete(unzipFolder, true);
        return true;
    }

    private int GetCurrentSystemEnumIndex()
    {
        if (OperatingSystem.IsWindows()) return 0;
        if (OperatingSystem.IsMacOS()) return 1;
        if (OperatingSystem.IsLinux()) return 2;
        throw new NotImplementedException();
    }

    [RelayCommand]
    private async Task InstallApp(AppListItemModel selected)
    {
        if (await InstallAsync(selected))
        {
            InstalledApps.Add(selected);
            AvailableApps.Remove(selected);
        }
    }

    [RelayCommand]
    private async Task Update(AppListItemModel selected)
    {
        if (await appStateService.AppIsRunningAsync(selected.AppName, selected.ExeName()))
        {
            await DialogServer.ShowMessageDialogAsync(
                string.Format(Language.RunningNotUpdate,
                    LocalizationResourceManager.Instance[selected.AppName.ToString()]), Language.Warning, Language.Ok);
            return;
        }

        if (await InstallAsync(selected))
        {
            // InstalledApps.Add(selected);
            // AvailableApps.Remove(selected);
        }
    }

    #region 工具

    private async Task LoadInstalledAppsAsync()
    {
        var apps = Enum.GetValues<AppName>();
        foreach (var app in apps)
            if (await appStateService.QueryAppExistsAsync(app))
            {
                var startFolder = await appStateService.QueryAppLocationAsync(app);
                var version = await appStateService.QueryAppInstalledVersionAsync(app);
                if (version is not null)
                    InstalledApps.Add(new AppListItemModel
                    {
                        AppName = app, InstallLocation = startFolder, Version = version, Status = AppStatus.Installed
                    });
                //Icon =  $"avares://TabbyCat/Assets/{a}.png"
            }
            else
            {
                var release = await appStateService.QueryLatestReleaseAsync(app);
                if (release is null)
                    continue;
                AvailableApps.Add(new AppListItemModel
                {
                    AppName = app, LatestVersion = release.Version, Description = release.Description,
                    Status = AppStatus.Available
                });
            }
    }

    private void LoopCheckAppRunningStatus()
    {
        while (true)
        {
            foreach (var item in InstalledApps)
                if (item.Status is AppStatus.Installed or AppStatus.Running)
                {
                    var result = appStateService.AppIsRunningAsync(item.AppName, item.AppName + ".Desktop").GetAwaiter()
                        .GetResult();
                    item.Status = result ? AppStatus.Running : AppStatus.Installed;
                }

            Thread.Sleep(TimeSpan.FromSeconds(2));
        }
    }

    private void LoopCheckVersion()
    {
        while (true)
        {
            var apps = Enum.GetValues<AppName>();
            foreach (var item in apps)
            {
                var result = appStateService.QueryLatestReleaseAsync(item).GetAwaiter().GetResult();
                if (result is null)
                    continue;
                var installed = InstalledApps.FirstOrDefault(x => x.AppName == item);
                if (installed is not null)
                {
                    if (installed.Status is AppStatus.Downloading or AppStatus.Installing or AppStatus.Waiting)
                        continue;

                    installed.LatestVersion = result.Version;

                    if (installed.LatestVersion != installed.Version)
                    {
                        installed.Description = result.Description;
                        installed.Status = AppStatus.NeedUpdate;
                    }
                }
                else
                {
                    var ava = AvailableApps.FirstOrDefault(x => x.AppName == item);
                    if (ava is not null)
                    {
                        if (ava.Status is AppStatus.Downloading or AppStatus.Installing or AppStatus.Waiting)
                            continue;
                        if (ava.LatestVersion != result.Version)
                        {
                            ava.LatestVersion = result.Version;
                            ava.Description = result.Description;
                        }
                    }
                }
            }

            Thread.Sleep(TimeSpan.FromSeconds(20));
        }
    }

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

    public bool Keep { get; } = true;
}