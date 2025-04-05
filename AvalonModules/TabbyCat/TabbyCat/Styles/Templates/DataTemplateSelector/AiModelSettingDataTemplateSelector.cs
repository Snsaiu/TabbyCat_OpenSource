using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using TabbyCat.Models;
using TabbyCat.Shared.Enums;

namespace TabbyCat.Styles.Templates.DataTemplateSelector;

public sealed class AiModelSettingDataTemplateSelector : IDataTemplate
{
    [Content] public Dictionary<string, IDataTemplate> AvailableTemplates { get; } = [];


    public Control? Build(object? param)
    {
        if (param is not { } p)
        {
            return null;
        }
        var key = p.GetType().Name;
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException($"{nameof(p)}");
        return AvailableTemplates[key].Build(p);
    }

    public bool Match(object? data)
    {
        if (data is null)
            return false;

        var key = data.GetType().Name;

        return data is AiApiModelBase && !string.IsNullOrEmpty(key) && AvailableTemplates.ContainsKey(key);
    }
}