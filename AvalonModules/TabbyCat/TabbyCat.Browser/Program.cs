﻿using Avalonia;
using Avalonia.Browser;

using System.Runtime.Versioning;
using System.Threading.Tasks;

using TabbyCat;

internal sealed partial class Program
{
    private static Task Main(string[] args) => BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}