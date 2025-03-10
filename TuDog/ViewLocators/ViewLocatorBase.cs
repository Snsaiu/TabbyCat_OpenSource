using Avalonia.Controls;
using Avalonia.Controls.Templates;
using TuDog.Bootstrap;

namespace TuDog.ViewLocators
{
    public abstract class ViewLocatorBase
    {
        public Control? Build(object? param)
        {
            var viewType = GetViewType(param);
            if (viewType is null)
                return ErrorView(param);

            return (Control)Activator.CreateInstance(viewType)!;
        }


        protected virtual Control ErrorView(object? param) => new TextBlock { Text = "Not Found: " + param };

        public abstract Type? GetViewType(object? param);

        protected abstract bool MatchViewModel(object? data);

        public bool Match(object? data) => MatchViewModel(data);
    }
}