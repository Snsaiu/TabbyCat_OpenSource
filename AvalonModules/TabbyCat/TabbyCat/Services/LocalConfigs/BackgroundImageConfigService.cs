using TabbyCat.Enums;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models.Users.Configs;
using TuDog.Interfaces.PreferenceServices.Impl;
using TuDog.IocAttribute;

namespace TabbyCat.Services.LocalConfigs;

[Register<IBackgroundImageConfigService>]
public sealed class BackgroundImageConfigService : LocalConfigService<BackgroundImageConfig>,
    IBackgroundImageConfigService
{
    public override string Key { get; } = "backgroundImageConfig";

    public override BackgroundImageConfig Default { get; } =
        new() { Opacity = 0.8, Status = BackgroundImageStatus.Default };
}