using CommunityToolkit.Mvvm.ComponentModel;
using TuDog.Bootstrap;

namespace TabbyCat.Models.Users;

public partial class LogoutOptionModel : ModelBase
{
    /// <summary>
    /// 清除Ai的所有key
    /// </summary>
    [ObservableProperty] private bool clearAiApiKeys = false;

    /// <summary>
    /// 清除所有的图片资源
    /// </summary>
    [ObservableProperty] private bool clearImageResource = false;

    /// <summary>
    /// 清除所有的聊天数据
    /// </summary>
    [ObservableProperty] private bool clearChats = false;
}