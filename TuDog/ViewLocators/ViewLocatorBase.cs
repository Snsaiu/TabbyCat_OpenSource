using Avalonia.Controls;

using Microsoft.Extensions.DependencyInjection;

using TuDog.Bootstrap;
using TuDog.Interfaces;

namespace TuDog.ViewLocators
{


    public abstract partial class ViewLocatorBase
    {

        protected ViewLocatorBase()
        {
            var service = TuDogApplication.ServiceProvider.GetRequiredService<IViewLocatorService>();
            _controlDictionary = service.InitControlDictionaryControls();

        }

        private readonly IDictionary<Type, Func<Control>>? _controlDictionary;

        public Control? Build(object? param)
        {
            var viewType = GetViewType(param);
            if (viewType is null)
                return ErrorView(param);

            if (_controlDictionary is null)
                return null;

            return _controlDictionary.TryGetValue(viewType, out Func<Control>? func) ? func() : null;
        }


        protected virtual Control ErrorView(object? param) => new TextBlock { Text = "Not Found: " + param };

        public abstract Type? GetViewType(object? param);

        protected abstract bool MatchViewModel(object? data);

        public bool Match(object? data) => MatchViewModel(data);
    }
}