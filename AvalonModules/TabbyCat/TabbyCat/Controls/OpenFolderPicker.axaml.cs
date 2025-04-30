using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Input;
using TabbyCat.Shared.Languages;

namespace TabbyCat.Controls;

public partial class OpenFolderPicker : UserControl
{
    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<OpenFolderPicker, string>(
        nameof(Title), AppResources.SelectFolder);

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly StyledProperty<string> SavePathProperty =
        AvaloniaProperty.Register<OpenFolderPicker, string>(
            nameof(SavePath), defaultBindingMode: BindingMode.TwoWay);

    public string SavePath
    {
        get => GetValue(SavePathProperty);
        set => SetValue(SavePathProperty, value);
    }

    public static readonly StyledProperty<string> PlaceHolderProperty =
        AvaloniaProperty.Register<OpenFolderPicker, string>(
            nameof(PlaceHolder), AppResources.OpenFolder);

    public string PlaceHolder
    {
        get => GetValue(PlaceHolderProperty);
        set => SetValue(PlaceHolderProperty, value);
    }

    public OpenFolderPicker()
    {
        InitializeComponent();
    }

    [RelayCommand]
    private async Task Open()
    {
        var top = TopLevel.GetTopLevel(this);
        if (top is null)
            throw new NullReferenceException();

        var result = await top.StorageProvider.OpenFolderPickerAsync(
            new() { AllowMultiple = false, Title = Title });
        if (result.Any()) SavePath = result.First().Path.LocalPath;
    }
}