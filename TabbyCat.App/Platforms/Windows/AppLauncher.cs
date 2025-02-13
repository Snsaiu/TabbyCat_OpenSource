using TabbyCat.App.Interfaces;
using System.Diagnostics;

namespace TabbyCat.App
{
    public class AppLauncher:IAppLauncher
    {
        public bool LaunchApp(string urlScheme)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = @"C:\Path\To\YourApp.exe", // 程序路径
                    UseShellExecute = true // 设置为 true 可使用默认应用打开文件或程序
                });
                return true
            }
            catch (Exception e)
            {
                return false;
            }
           
        }
        
    }
}