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
            var programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            return Path.Combine(programFilesX86, "YouYan");
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

        throw new NotImplementedException();
    }
}