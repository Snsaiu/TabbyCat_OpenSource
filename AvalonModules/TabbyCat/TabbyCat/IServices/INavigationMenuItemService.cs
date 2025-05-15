using TabbyCat.Models;

namespace TabbyCat.IServices;

public interface INavigationMenuItemService
{
    IEnumerable<NavigationMenuItem> MenuItems { get; }
    NavigationMenuItem SelectMenuItem { get; set; }

    Action<NavigationMenuItem> SelectMenuItemAction { get; set; }
}