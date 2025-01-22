using AirTransfer.Interfaces;
using AirTransfer.Native;

using PInvoke;

using System.Runtime.InteropServices;

using TabbyCat.Shared;

namespace AirTransfer;

public sealed class TrayService : ITrayService
{
    private WindowsTrayIcon? tray;

    public Action? ClickHandler { get; set; }

    public void Initialize()
    {
        tray = new("Platforms\\Windows\\trayicon.ico");
        tray.LeftClick = () =>
        {
            WindowExtensions.BringToFront();
            ClickHandler?.Invoke();
        };
        tray.RightClick = () => { ShowContextMenu(); };
    }

    private void ShowContextMenu()
    {
        var quitProgreamText = LocalizationResourceManager.Instance["ExitTheApplication"];
        var hMenu = CreatePopupMenu();
        // AppendMenu(hMenu, MF_STRING, 1, "Option 1");
        // AppendMenu(hMenu, MF_STRING, 2, "Option 2");
        // AppendMenu(hMenu, MF_SEPARATOR, 0, string.Empty);
        AppendMenu(hMenu, MF_STRING, 1, quitProgreamText);

        POINT pt;
        GetCursorPos(out pt);
        SetForegroundWindow(tray.messageSink.MessageWindowHandle);
        var cmd = TrackPopupMenu(hMenu, TPM_RETURNCMD, pt.x, pt.y, 0, tray.messageSink.MessageWindowHandle,
            IntPtr.Zero);
        if (cmd == 1)
            Application.Current?.Quit();
    }

    private void HandleOption1()
    {
        // Handle Option 1
    }

    private void HandleOption2()
    {
        // Handle Option 2
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr CreatePopupMenu();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool AppendMenu(IntPtr hMenu, uint uFlags, uint uIDNewItem, string lpNewItem);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd,
        IntPtr prcRect);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    private const uint MF_STRING = 0x00000000;
    private const uint MF_SEPARATOR = 0x00000800;
    private const uint TPM_RETURNCMD = 0x0100;
}