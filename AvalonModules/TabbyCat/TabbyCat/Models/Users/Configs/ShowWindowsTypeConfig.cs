using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.Enums;
using TabbyCat.IServices;
using TuDog.Bootstrap;
using TuDog.IocAttribute;

namespace TabbyCat.Models.Users.Configs;

[Register<IShowWindowTypeConfig>(ServiceLifetime.Singleton)]
public partial class ShowWindowsTypeConfig : ModelBase, IShowWindowTypeConfig
{
    [ObservableProperty] private WindowsShowType _windowsShowType = WindowsShowType.MainWindow;

    public Action<WindowsShowType>? ChangedCallBack { get; set; }

    partial void OnWindowsShowTypeChanged(WindowsShowType value)
    {
        ChangedCallBack?.Invoke(value);
    }
}