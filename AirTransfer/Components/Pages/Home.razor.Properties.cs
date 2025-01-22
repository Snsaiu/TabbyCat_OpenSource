#region

using AirTransfer.Interfaces;
using AirTransfer.Interfaces.Impls;
using AirTransfer.Interfaces.Impls.TcpTransfer;
using AirTransfer.Interfaces.Impls.UdpTransfer;
using AirTransfer.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;

using TabbyCat.Service.LocalNetShareServices;
using TabbyCat.Shared.Interfaces;

#endregion

namespace AirTransfer.Components.Pages;

public partial class Home
{
    private readonly CancellationTokenSource _cancelDownloadTokenSource = new();

    private string DeviceNickName = string.Empty;


    private bool _isDownLoadingVisible;

    private bool _newMessageVisible;

    private string UserName = string.Empty;

    private DeviceModel _localDevice = null!;

    #region Injects

    [Inject] private ILogger<Home> Logger { get; set; } = null!;
    [Inject] private ISaveDataService DataService { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private LocalNetDeviceDiscoveryBase LocalNetDeviceDiscoveryBase { get; set; } = null!;
    [Inject] private LocalNetInviteDeviceBase LocalNetInviteDeviceBase { get; set; } = null!;
    [Inject] private DeviceLocalIpBase DeviceLocalIpBase { get; set; } = null!;
    [Inject] private LocalIpScannerBase LocalIpScannerBase { get; set; } = null!;
    [Inject] private LocalNetJoinRequestBase LocalNetJoinRequestBase { get; set; } = null!;
    [Inject] private LocalNetJoinProcessBase LocalNetJoinProcessBase { get; set; } = null!;
    [Inject] private TcpLoopListenContentBase TcpLoopListenContentBase { get; set; } = null!;
    [Inject] private TcpSendFileBase TcpSendFileBase { get; set; } = null!;
    [Inject] private TcpSendTextBase TcpSendTextBase { get; set; } = null!;
    [Inject] private IDeviceType DeviceType { get; set; } = null!;
    [Inject] private IPortCheckable PortCheckable { get; set; } = null!;

    [Inject] private IUserService UserService { get; set; } = null!;

    [Inject] private IClipboardWatchable ClipboardWatchable { get; set; } = null!;

    #endregion
}