using CommunityToolkit.Mvvm.ComponentModel;
using TuDog.Bootstrap;
using TuDog.Interfaces;

namespace TuDog.UIs;

public partial class InputTextViewModel : ParameterViewModelBase, IViewModelResult
{
    [ObservableProperty] private string watermark = string.Empty;

    [ObservableProperty] private string text = string.Empty;


    public object Confirm()
    {
        return Text;
    }

    public object Cancel()
    {
        return string.Empty;
    }
}