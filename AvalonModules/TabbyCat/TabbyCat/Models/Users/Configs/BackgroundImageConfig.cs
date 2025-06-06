using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.Enums;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TuDog.Bootstrap;
using TuDog.IocAttribute;

namespace TabbyCat.Models.Users.Configs;

[Register<IBackgroundImageConfig>(ServiceLifetime.Singleton)]
public sealed partial class BackgroundImageConfig : ModelBase, IBackgroundImageConfig
{
    [ObservableProperty] private BackgroundImageStatus _status;

    [ObservableProperty] private string? _customImage;

    [ObservableProperty] private double _opacity = 0.8;

    public BackgroundImageConfig()
    {
        if (Status == BackgroundImageStatus.Custom && !File.Exists(CustomImage))
        {
            CustomImage = string.Empty;
            Status = BackgroundImageStatus.Disabled;
        }
    }
}