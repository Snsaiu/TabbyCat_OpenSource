using System.Runtime.InteropServices;

namespace TabbyCat.App.Native;

[StructLayout(LayoutKind.Sequential)]
public struct Point
{
    /// <summary>
    /// X coordinate.
    /// </summary>
    public int X;

    /// <summary>
    /// Y coordinate.
    /// </summary>
    public int Y;
}