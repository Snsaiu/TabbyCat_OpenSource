namespace TuDog.Interfaces.MessageBarService;

public interface IMessageBarService
{
    public void ShowSuccess(string message,string title,bool showClose);

    public void ShowError(string message, string title, bool showClose);

    public void ShowWarning(string message, string title, bool showClose);
}