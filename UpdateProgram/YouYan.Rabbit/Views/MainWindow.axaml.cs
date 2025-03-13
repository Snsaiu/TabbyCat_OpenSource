using Avalonia.Interactivity;

using FluentAvalonia.UI.Windowing;

using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

using TuDog.Bootstrap;
using TuDog.Interfaces.RegionManagers;

using YouYan.Rabbit.ViewModels;

namespace YouYan.Rabbit.Views;

public partial class MainWindow : AppWindow
{
    public MainWindow()
    {
        InitializeComponent();
        TitleBar.ExtendsContentIntoTitleBar = true;
    }



    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {

        var regionManager = TuDogApplication.ServiceProvider.GetService<IRegionManager>();
        regionManager.AddToRegion<AppListViewModel>("container");
    }
}