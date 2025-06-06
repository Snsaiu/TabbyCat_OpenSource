using TabbyCat.Shared.Enums;

namespace TabbyCat.ViewModels.Bases;

public interface IMediaNavigation
{
    Task NavigationAsync(object? parameter);
}