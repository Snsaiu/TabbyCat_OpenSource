using System;
using System.Diagnostics;

namespace YouYan.Rabbit.Extensions;

public static class MacOsHelper
{
    /// <summary>
    /// 移除指定 .app 文件的 quarantine 标志
    /// </summary>
    /// <param name="appPath">.app 文件路径</param>
    /// <returns>是否成功</returns>
    public static bool RemoveQuarantine(string appPath)
    {
        if (string.IsNullOrWhiteSpace(appPath))
        {
            throw new ArgumentException("路径无效");
        }


        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "xattr",
            Arguments = $"-r -d com.apple.quarantine \"{appPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = new Process { StartInfo = psi })
        {
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}