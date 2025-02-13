namespace TabbyCat.App.Interfaces;

public interface ITrayService
{
    void Initialize();

    Action ClickHandler { get; set; }
}