using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace YouYan.Rabbit.ViewModels;

public abstract partial class ConfirmViewModelBase : ViewModelBase
{
    [RelayCommand]
    private Task Confirm()
    {
        return OnConfirm();
    }

    protected abstract Task OnConfirm();
}