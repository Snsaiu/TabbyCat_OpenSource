using Avalonia.Markup.Xaml;
using HotAvalonia;
using Microsoft.Extensions.DependencyInjection;

using TuDog.Bootstrap;

using YouYan.Rabbit.Views;

namespace YouYan.Rabbit;

public partial class App : TuDogApplication
{
    public override object CreateShell()
    {
        return new MainWindow();
    }

    public override void Initialize()
    {
        this.EnableHotReload();
        AvaloniaXamlLoader.Load(this);
        base.Initialize();
    }

    protected override void Register(IServiceCollection collection)
    {
        collection.AddHttpClient();
    }


}