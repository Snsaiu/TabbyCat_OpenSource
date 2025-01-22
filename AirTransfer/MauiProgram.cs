using AirTransfer.Extensions;
using AirTransfer.Interfaces;
using AirTransfer.Interfaces.IConfigs;
using AirTransfer.Interfaces.Impls;
using AirTransfer.Interfaces.Impls.Configs;
using AirTransfer.Interfaces.Impls.TcpTransfer;
using AirTransfer.Interfaces.Impls.UdpTransfer;

using CommunityToolkit.Maui;

using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;

using TabbyCat.Service.AiServices;
using TabbyCat.Service.LocalNetShareServices;
using TabbyCat.Shared.Interfaces;
using TabbyCat.SqliteService.AiServices;
using TabbyCat.SqliteService.LocalNetShareServices;


namespace AirTransfer
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddFluentUIComponents(options => { options.UseTooltipServiceProvider = true; });
            builder.Services.AddLocalization();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<IUserService, ConfigUserService>();
            builder.Services.AddSingleton<IStateManager, StateManager>();


            builder.Services.AddSingleton<IOpenFolder, DefaultOpenFolder>();
            builder.Services.AddSingleton<IFileSavePath, FileSavePath>();

            builder.Services.AddSingleton<LocalNetDeviceDiscoveryBase, LocalNetDeviceDiscovery>();
            builder.Services.AddSingleton<LocalNetInviteDeviceBase, LocalNetInviteDevice>();
            builder.Services.AddSingleton<LocalIpScannerBase, DefaultScanLocalNetIp>();
            builder.Services.AddSingleton<LocalNetJoinRequestBase, LocalNetJoinRequest>();
            builder.Services.AddSingleton<FileSavePathBase, FileSavePath>();
            builder.Services.AddSingleton<ISavePathService, SavePathService>();


            builder.Services.AddSingleton<DeviceLocalIpBase, DefaultLocalIp>();

            builder.Services.AddSingleton<ISystemType, SystemTypeProvider>();
            builder.Services.AddSingleton<IDeviceType, DeviceTypeProvider>();

            builder.Services.AddSingleton<IOpenFileable, OpenFileProvider>();

            builder.Services.AddSingleton<LocalNetJoinProcessBase, LocalNetJoinProcess>();
            builder.Services.AddSingleton<GlobalScanBase, GlobalScan>();

            builder.Services.AddSingleton<TcpSendFileBase, TcpSendFile>();
            builder.Services.AddSingleton<TcpSendTextBase, TcpSendText>();


            builder.Services.AddSingleton<TcpLoopListenContentBase, TcpLoopListenContent>();

            // builder.Services.AddSingleton<IGetLocalNetDevices, DefaultScanLocalNetIp>();

            #region DBService
            builder.Services.AddSingleton<IAiChatSessionService, AiChatSessionService>();
            builder.Services.AddSingleton<IAiChatMessageRecordService, AiChatMessageRecordService>();
            builder.Services.AddSingleton<ISaveDataService, DbSaveDataService>();
            builder.Services.AddSingleton<IAiTemplateSettingService, AiTemplateSettingService>();
            builder.Services.AddSingleton<ICustomAssistantOccupationService,CustomAssistantOccupationService>();
            #endregion

            builder.Services.AddSingleton<ILanguageService, ConfigLanguageService>();
            builder.Services.AddSingleton<ISendPortService, SendPortService>();
            builder.Services.AddSingleton<IPortCheckable, PortChecker>();

            builder.Services.AddSingleton<IThemeService, ThemeService>();
            builder.Services.AddSingleton<IClipboardWatchable, ClipboardWatcher>();
            builder.Services.AddSingleton<ILoopWatchClipboardService, LoopWatchClipboardService>();
            builder.Services.AddSingleton<IDialogService, DialogService>();



#if MACCATALYST || WINDOWS
            builder.Services.AddSingleton<ITrayService, TrayService>();
            builder.Services.AddSingleton<IShowCloseDialogService, ShowCloseDialogService>();
            builder.Services.AddSingleton<ICloseAppBehaviorService, CloseAppBehaviorService>();
            builder.Services.AddSingleton<ITopMostService, TopMostService>();

            builder.Services.AddSingleton<IHotKeyHookService, HotKeyHookService>();
#endif
            builder.Services.AddHttpClient();


            builder.ConfigureLifecycle();

            return builder.Build();
        }
    }
}