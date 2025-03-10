using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;

using Microsoft.Extensions.DependencyInjection;

using TuDog.Bases.Regions;
using TuDog.Bases.Regions.Impl;
using TuDog.Interfaces.IDialogServers;
using TuDog.Interfaces.IDialogServers.Impl;
using TuDog.Interfaces.PreferenceServices;
using TuDog.Interfaces.PreferenceServices.Impl;
using TuDog.Interfaces.RegionManagers;
using TuDog.Interfaces.RegionManagers.Impl;
using TuDog.IocContainers;
using TuDog.IocContainers.Impl;
using TuDog.ViewLocators;
using TuDog.ViewLocators.Impl;

namespace TuDog.Bootstrap;

public abstract class TuDogApplication : Application
{
    public static IServiceProvider? ServiceProvider { get; private set; }

    public static TopLevel? TopLevel { get; set; }

    public static Window? MainWindow { get; private set; }

    public override void Initialize()
    {
        var collection = new ServiceCollection();
        SystemServiceRegister(collection);
        Register(collection);
        AutoRegister(collection);
        ServiceProvider = collection.BuildServiceProvider();
    }

    protected virtual void AutoRegister(IServiceCollection collection)
    {
    }

    protected void SystemServiceRegister(IServiceCollection collection)
    {
        collection.AddSingleton<ViewLocatorBase, ViewLocator>();
        collection.AddSingleton<RegionContainerBase, RegionContainer>();
        collection.AddSingleton<IContainer, Container>();
        collection.AddSingleton<IRegionManager, RegionManager>();
        collection.AddSingleton<IDialogServer, DialogServer>();
        collection.AddSingleton<IPreferenceService, JsonPreferenceService>();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    public abstract object CreateShell();

    public override void OnFrameworkInitializationCompleted()
    {


        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = (Window)CreateShell();
            MainWindow = window;
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit.
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = window;
            TopLevel = TopLevel.GetTopLevel(desktop.MainWindow);
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            var window = CreateShell();
            //MainWindow =window;
            singleViewPlatform.MainView = (UserControl)window;
            TopLevel = TopLevel.GetTopLevel(singleViewPlatform.MainView);
        }

        base.OnFrameworkInitializationCompleted();
    }


    protected virtual void Register(IServiceCollection collection)
    {
    }
}