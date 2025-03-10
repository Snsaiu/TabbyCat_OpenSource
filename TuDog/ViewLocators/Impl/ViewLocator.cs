using Avalonia.Controls;
using TuDog.Bootstrap;

namespace TuDog.ViewLocators.Impl;

public class ViewLocator:ViewLocatorBase
{
    public override Type? GetViewType(object? param)
    {
        if (param is null)
            return null;

        var name = param.ToString()!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var ass= name.Split(".Views").First();
        if (ass.Contains(".")) ass = ass.Split(".").First();
        return Type.GetType($"{name},{ass}");
    }

    protected override bool MatchViewModel(object? data) => true;
}