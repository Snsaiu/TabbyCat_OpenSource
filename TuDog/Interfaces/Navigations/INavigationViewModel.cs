namespace TuDog.Interfaces.Navigations;

public interface INavigationViewModel
{
    Task OnPushHereAsync(INavigationParameter? parameter);
    
    Task OnPopHereAsync(INavigationParameter? result);
}