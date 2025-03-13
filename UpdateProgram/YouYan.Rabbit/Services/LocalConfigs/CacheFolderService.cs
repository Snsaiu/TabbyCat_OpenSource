using System;
using System.IO;
using TuDog.Interfaces.PreferenceServices.Impl;
using TuDog.IocAttribute;
using YouYan.Rabbit.IServices.LocalConfigs;

namespace YouYan.Rabbit.Services.LocalConfigs;

[Register<ICacheFolderService>]
public sealed class CacheFolderService : LocalConfigService<string>, ICacheFolderService
{
    public override string Key { get; } = "cacheFolder";

    public override string Default => GetCacheDirectory("Rabbit");

    private string GetCacheDirectory(string appName)
    {
        var baseCachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        var cachePath = Path.Combine(baseCachePath, appName, "Cache");

        if (!Directory.Exists(cachePath)) Directory.CreateDirectory(cachePath);

        return cachePath;
    }
}