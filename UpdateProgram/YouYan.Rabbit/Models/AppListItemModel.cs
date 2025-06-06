using System;
using CommunityToolkit.Mvvm.ComponentModel;
using TuDog.Bootstrap;
using YouYan.Rabbit.Extensions;

namespace YouYan.Rabbit.Models;

public partial class AppListItemModel : ModelBase
{
    [ObservableProperty] private AppName appName;

    [ObservableProperty] private string version = string.Empty;

    [ObservableProperty] private string icon = string.Empty;

    [ObservableProperty] private AppStatus status = AppStatus.Available;

    [ObservableProperty] private string description = string.Empty;

    [ObservableProperty] private double downloadProgress = 0;

    [ObservableProperty] private string installLocation = string.Empty;

    [ObservableProperty]
    private string latestVersion = string.Empty;

    [ObservableProperty] private string releaseNotes = string.Empty;


    public  string ExeName()
    {
        return appName switch
        {
            AppName.TabbyCat => AppName.ToString()+ ".Desktop",
            AppName.CowCat => AppName.ToString()+".Desktop",
            _ => throw new ArgumentOutOfRangeException()
        };
    }


}