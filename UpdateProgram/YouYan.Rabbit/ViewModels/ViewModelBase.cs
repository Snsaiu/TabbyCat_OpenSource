using Microsoft.Extensions.DependencyInjection;
using TuDog.Bootstrap;
using TuDog.Interfaces.IDialogServers;

namespace YouYan.Rabbit.ViewModels;

public abstract partial class ViewModelBase : TuDogViewModelBase
{
    protected IDialogServer DialogServer { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IDialogServer>();

}