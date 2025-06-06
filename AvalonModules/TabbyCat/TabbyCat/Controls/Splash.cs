using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

using FluentAvalonia.UI.Windowing;

using System.Threading;

using TabbyCat.Shared.Languages;

namespace TabbyCat.Controls;

public sealed class Splash : IApplicationSplashScreen
{
    public Task RunTasks(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public string AppName { get; } = AppResources.TabbyCat;

    public IImage AppIcon { get; } = new Bitmap(AssetLoader.Open(new Uri("avares://TabbyCat/Assets/logo.png")));
    public object SplashScreenContent { get; } = new TextBlock() { Text = "hello" };
    public int MinimumShowTime { get; } = 2000;
}