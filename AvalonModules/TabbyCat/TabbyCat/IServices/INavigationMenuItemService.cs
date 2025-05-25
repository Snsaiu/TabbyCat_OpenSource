using TabbyCat.Models;
using TabbyCat.Shared.Enums;

namespace TabbyCat.IServices;

public interface INavigationMenuItemService
{
    IEnumerable<NavigationMenuItem> MenuItems { get; }
    NavigationMenuItem SelectMenuItem { get; set; }

    Action<NavigationMenuItem> SelectMenuItemAction { get; set; }
    
    object? Parameter { get; set; }
    
    public Task NavigationAsync(AiMediaWorkType aiMediaWorkType,object? parameter);
}