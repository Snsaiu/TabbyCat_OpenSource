namespace TuDog.Interfaces;

public interface IViewLocatorService
{
    IDictionary<Type, Func<Avalonia.Controls.Control>> InitControlDictionaryControls();
}
