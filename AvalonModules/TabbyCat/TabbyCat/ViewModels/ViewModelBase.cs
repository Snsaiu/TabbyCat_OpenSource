using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.Shared;
using TuDog.Bootstrap;
using TuDog.Interfaces.IDialogServers;

namespace TabbyCat.ViewModels
{
    public abstract partial class ViewModelBase : TuDogViewModelBase
    {
        protected IDialogServer DialogServer { get; } =
            TuDogApplication.ServiceProvider.GetRequiredService<IDialogServer>();

        protected LocalizationResourceManager ResourceManager { get; } = LocalizationResourceManager.Instance;

    }
}