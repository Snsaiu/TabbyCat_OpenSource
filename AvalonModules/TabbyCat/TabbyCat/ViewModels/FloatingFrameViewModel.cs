using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TabbyCat.Enums;
using TabbyCat.Views;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public partial class FloatingFrameViewModel : ChatViewModel
{
    [ObservableProperty] private bool showOutputPanel = false;

    [ObservableProperty] private bool toEnd = true;

    public Action OnAiResponseChanged { get; set; }

    protected override Task BeforeSendAsync(string content)
    {
        ShowOutputPanel = true;
        ToEnd = true;
        return Task.CompletedTask;
    }

    [RelayCommand]
    private void ShowOutPutPanel()
    {
        ShowOutputPanel = !ShowOutputPanel;
    }

    [RelayCommand]
    private void ClearInput()
    {
        InputTextContent = "";
    }

    [RelayCommand]
    private void ToggleWindowType()
    {
        ShowWindowTypeConfig.WindowsShowType = WindowsShowType.MainWindow;
        RegionManager.AddToRegion<MainViewModel>("mainContainer");
    }


    protected override void OnAiResponseCharacter(string content)
    {
        OnAiResponseChanged?.Invoke();
    }
}