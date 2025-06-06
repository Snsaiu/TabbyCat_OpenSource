using Avalonia.Controls;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using TuDog.Bootstrap;
using TuDog.Enums;
using TuDog.Extensions;
using TuDog.IocContainers;
using TuDog.Models;
using TuDog.UIs;
using TuDog.ViewLocators;

namespace TuDog.Interfaces.IDialogServers.Impl;

public class DialogServer(ViewLocatorBase viewLocatorBase, IContainer container) : IDialogServer
{
    public Task ShowMessageDialogAsync(string message, string title = "提示",
        string buttonText = "确定")
    {
        var dialog = new ContentDialog()
        {
            Title = title,
            Content = message,
            PrimaryButtonText = buttonText
        };
        return dialog.ShowAsync();
    }

    public async Task<bool> ShowConfirmDialogAsync(string message, string title = "提示", string confirmButtonText = "确定",
        string cancelButtonText = "取消")
    {
        var dialog = new ContentDialog()
        {
            Title = title,
            Content = message,
            PrimaryButtonText = confirmButtonText,
            SecondaryButtonText = cancelButtonText
        };
        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }

    public async Task<DialogResultData<string>> ShowInputDialogAsync(string message, string title = "提示",
        string placeHolder = "请输入...",
        string confirmButtonText = "确定",
        string cancelButtonText = "取消", string? defaultValue = null)
    {
        var dialog = new ContentDialog()
        {
            Title = title,
            PrimaryButtonText = confirmButtonText,
            SecondaryButtonText = cancelButtonText
        };

        var vm = new InputTextViewModel
        {
            Text = message,
            Watermark = placeHolder
        };
        var control = new InputTextView()
        {
            DataContext = vm
        };
        dialog.Content = control;
        var dialogResult = await dialog.ShowAsync();
        if (dialogResult == ContentDialogResult.Primary) return new(true, vm.Confirm()?.ToString() ?? string.Empty);
        return new(false, string.Empty);
    }

    public async Task<DialogResultData<TResult>?> ShowDialogAsync<TViewModel,TParameter, TResult>(string title,
        string confirmButtonText = "确定",
        string cancelButtonText = "取消", TParameter? parameter = default)
        where TViewModel : DialogViewModelBaseAsync<TParameter,TResult>
    {
        var vm = container.GetRequiredService<TViewModel>();
        if (vm is null)
            throw new ArgumentNullException();

        vm.Parameter = parameter;

        var view = viewLocatorBase.Build(vm);
        if (view is null)
            throw new ArgumentNullException();

        view.DataContext = vm;
        view.AttachLoadedBehavior(vm);
        view.AttachUnLoadedBehavior(vm);

     
        var dialog = new DialogWindow()
        {
            Title = title,
            PrimaryButtonText = confirmButtonText,
            SecondaryButtonText = cancelButtonText,
            Content = view,
            DialogViewModel = vm
        };
      
        var result = await dialog.ShowDialog<DialogResultData>(TuDogApplication.MainWindow);
        return result;
    }

    public async Task<DialogResultData<object>?> ShowDialogAsync<TViewModel, TParameter>(string title, string confirmButtonText = "确定",
        string cancelButtonText = "取消", TParameter? parameter = default) where TViewModel : DialogViewModelBaseAsync
    {
        var vm = container.GetRequiredService<TViewModel>();
        if (vm is null)
            throw new ArgumentNullException();

        vm.Parameter = parameter;

        var view = viewLocatorBase.Build(vm);
        if (view is null)
            throw new ArgumentNullException();

        view.DataContext = vm;
        view.AttachLoadedBehavior(vm);
        view.AttachUnLoadedBehavior(vm);

     
        var dialog = new DialogWindow()
        {
            Title = title,
            PrimaryButtonText = confirmButtonText,
            SecondaryButtonText = cancelButtonText,
            Content = view,
            DialogViewModel = vm
        };
      
        var result = await dialog.ShowDialog<DialogResultData>(TuDogApplication.MainWindow);
        return result;
    }
}