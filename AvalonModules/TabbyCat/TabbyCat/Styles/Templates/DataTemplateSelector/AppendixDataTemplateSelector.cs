using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Metadata;
using TabbyCat.Models;

namespace TabbyCat.Styles.Templates.DataTemplateSelector;

public sealed class AppendixDataTemplateSelector:IDataTemplate
{

    [Content] public Dictionary<string, DataTemplate> DataTemplates { get; } = [];
    
    public Control? Build(object? param)
    {
        return param is not AppendixModel model ? null : DataTemplates[model.AppendixType.ToString()].Build(model);
    }

    public bool Match(object? data)
    {
        if (data is not AppendixModel { AppendixType: var appendixType })
        {
            return false;
        }

        var key = appendixType.ToString();
        
        return DataTemplates.ContainsKey(key);
    }
}