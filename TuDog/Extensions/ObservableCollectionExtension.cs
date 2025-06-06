using System.Collections.ObjectModel;
using Avalonia.Threading;

namespace TuDog.Extensions;

public static class ObservableCollectionExtension
{
    private static readonly Lock _lock = new();

    public static ObservableCollection<T> Reset<T>(this ObservableCollection<T> collection)
    {
        lock (_lock)
        {
            collection.Clear();
            return collection;
        }
    }

    public static ObservableCollection<T> Reset<T>(this ObservableCollection<T> collection, IEnumerable<T> values)
    {
        lock (_lock)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                collection.Clear();
                foreach (var value in values) collection.Add(value);
            });
            return collection;
        }
    }
}