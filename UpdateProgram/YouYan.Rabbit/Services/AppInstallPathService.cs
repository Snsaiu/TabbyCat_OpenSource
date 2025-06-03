using System;
using System.IO;
using TuDog.IocAttribute;
using YouYan.Rabbit.IServices;

namespace YouYan.Rabbit.Services;

[Register<IAppInstallPathService>]
public sealed class AppInstallPathService : IAppInstallPathService
{
    public string GetAppInstallPath()
    {
        if (OperatingSystem.IsWindows())
        {
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(localAppDataPath, "YouYan");
        }
        else if (OperatingSystem.IsLinux())
        {
        }
        else if (OperatingSystem.IsMacOS())
        {
            var userApplicationsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Applications");
            return Path.Combine(userApplicationsPath, "YouYan");
        }
        else
        {
            throw new NotImplementedException();
        }

        throw new NotImplementedException();
    }

    public string GetRabbitInstallPath()
    {
        if (OperatingSystem.IsWindows())
        {
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            return Path.Combine(localAppDataPath, "YouYan");
        }
        else if (OperatingSystem.IsLinux())
        {
        }
        else if (OperatingSystem.IsMacOS())
        {
            return "/Applications/Rabbit.app/Contents/MacOS";
        }
        else
        {
            throw new NotImplementedException();
        }

        throw new NotImplementedException();
    }
}