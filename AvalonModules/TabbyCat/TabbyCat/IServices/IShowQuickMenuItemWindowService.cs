using Avalonia;

namespace TabbyCat.IServices;

public interface IShowQuickMenuItemWindowService
{
    
    Task ShowWindowAsync(string content,Point position);
    
    Task HideWindowAsync(Point position);
    
    Task HideWindowAsync();
}