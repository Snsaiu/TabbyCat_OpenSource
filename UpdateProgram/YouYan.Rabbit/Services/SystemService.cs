using System;
using System.Runtime.InteropServices;

using TuDog.IocAttribute;
using YouYan.Rabbit.Extensions;
using YouYan.Rabbit.IServices;

namespace YouYan.Rabbit.Services
{
    [Register<ISystemService>]
    public sealed class SystemService : ISystemService
    {
        public string SystemBit
        {
            get
            {
                if (RuntimeInformation.OSArchitecture == Architecture.X64 ||
                    RuntimeInformation.OSArchitecture == Architecture.Arm64)
                {
                    return "64";
                }
                else if (RuntimeInformation.OSArchitecture == Architecture.X86 ||
                         RuntimeInformation.OSArchitecture == Architecture.Arm)
                {
                    return "32";
                }
                else
                {
                    return "";
                }
            }
        }

        public string? OSArchitecture => RuntimeInformation.OSArchitecture.ToString();

        public string? OSPlatform => RuntimeInformation.OSDescription;

        public AppOsType OsType
        {
            get
            {
                if (OperatingSystem.IsWindows())
                    return AppOsType.Windows;
                else if (OperatingSystem.IsMacOS())
                    return AppOsType.MacOs;
                else if (OperatingSystem.IsLinux())
                    return AppOsType.Ubuntu;
                throw new PlatformNotSupportedException();
            }
        }
    }
}