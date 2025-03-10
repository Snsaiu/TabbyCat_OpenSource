namespace TuDog.Interfaces.Navigations;

public interface INavigationService
{
    Task NavigationToAsync(string viewModelName);
}