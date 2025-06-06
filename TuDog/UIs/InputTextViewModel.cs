using CommunityToolkit.Mvvm.ComponentModel;
using TuDog.Bootstrap;
using TuDog.Interfaces;

namespace TuDog.UIs;

public partial class InputTextViewModel : DialogViewModelBase
{
    [ObservableProperty] private string _watermark = string.Empty;

    [ObservableProperty] private string _text = string.Empty;

    
    public override object Confirm()
    {
        return Text;
    }

    public override object Cancel()
    {
        return string.Empty;
    }
}