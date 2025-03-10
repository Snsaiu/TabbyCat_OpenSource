using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using TabbyCat.Models;
using TabbyCat.Shared.Enums;

namespace TabbyCat.Styles.Templates.DataTemplateSelector;

public sealed class AiModelSettingDataTemplateSelector : IDataTemplate
{
    [Content] public Dictionary<string, IDataTemplate> AvailableTemplates { get; } = new();


    public Control? Build(object? param)
    {
        var key = param.GetType().Name;
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException($"{nameof(param)}");
        return AvailableTemplates[key].Build(param);
    }

    public bool Match(object? data)
    {
        if (data is null)
            return false;

        var key = data.GetType().Name;

        return data is AiApiModelBase && !string.IsNullOrEmpty(key) && AvailableTemplates.ContainsKey(key);
    }
}