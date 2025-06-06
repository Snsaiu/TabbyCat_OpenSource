namespace TuDog.ViewLocators.Impl;

public class ViewLocator : ViewLocatorBase
{
    public override Type? GetViewType(object? param)
    {
        if (param is null)
            return null;

        var name = param.ToString()!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var ass = param.GetType().Assembly.GetName().Name;
        return Type.GetType($"{name},{ass}");
    }

    protected override bool MatchViewModel(object? data) => true;
}