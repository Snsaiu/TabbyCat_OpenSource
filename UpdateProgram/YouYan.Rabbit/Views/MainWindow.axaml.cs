using Avalonia.Interactivity;

using FluentAvalonia.UI.Windowing;

using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using TuDog.Bootstrap;
using TuDog.Interfaces.RegionManagers;
using Avalonia.VisualTree;

using YouYan.Rabbit.ViewModels;

namespace YouYan.Rabbit.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {

        InitializeComponent();

    }


    public bool CanExit()
    {
        return (((mainControl as ContentControl).Content as AppListView).DataContext as AppListViewModel).CanExit();
    }

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        Hide();
        var regionManager = TuDogApplication.ServiceProvider.GetService<IRegionManager>();
        regionManager.AddToRegion<AppListViewModel>("container");

    }


}