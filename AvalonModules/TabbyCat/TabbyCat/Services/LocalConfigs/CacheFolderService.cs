using System.IO;
using TabbyCat.IServices.LocalConfigs;
using TuDog.Interfaces.PreferenceServices.Impl;
using TuDog.IocAttribute;

namespace TabbyCat.Services.LocalConfigs;

[Register<ICacheFolderService>]
public sealed class CacheFolderService:LocalConfigService<string>,ICacheFolderService
{
    public override string Key => "cacheFolder";
    public override string Default =>GetCacheDirectory("TabbyCat");
    
    private static string GetCacheDirectory(string appName)
    {
        var baseCachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        var cachePath = Path.Combine(baseCachePath, appName, "Cache");

        if (!Directory.Exists(cachePath)) Directory.CreateDirectory(cachePath);

        return cachePath;
    }
}