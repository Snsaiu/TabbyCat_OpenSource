using AirTransfer.Interfaces;

using TabbyCat.Shared.Interfaces;


namespace AirTransfer.Models;

/// <summary>
/// 用户信息
/// </summary>
public class UserInfo : IName
{
    /// <summary>
    /// 用户名（唯一编号）
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 设备昵称
    /// </summary>
    public string DeviceNickName { get; set; } = string.Empty;
}