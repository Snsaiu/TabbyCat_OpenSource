using TuDog.Bootstrap;

namespace TuDog.Interfaces.Navigations;

public interface INavigationService
{
    Task PushAsync<ViewModel>(INavigationParameter? parameter) where ViewModel : TuDogViewModelBase;

    Task PopAsync(INavigationParameter? result=null);
}