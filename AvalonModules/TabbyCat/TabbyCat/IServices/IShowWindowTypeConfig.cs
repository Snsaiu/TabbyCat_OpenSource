using TabbyCat.Enums;

namespace TabbyCat.IServices;

public interface IShowWindowTypeConfig
{
    WindowsShowType WindowsShowType { get; set; }

    Action<WindowsShowType>? ChangedCallBack { get; set; }
}