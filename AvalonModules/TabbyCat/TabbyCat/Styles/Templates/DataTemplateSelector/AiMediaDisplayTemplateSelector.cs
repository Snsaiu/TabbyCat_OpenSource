using System.IO;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace TabbyCat.Styles.Templates.DataTemplateSelector;

public sealed class AiMediaDisplayTemplateSelector:IDataTemplate
{
    
    [Content]
    public Dictionary<string,IDataTemplate> Templates { get; } = [];
    
    public Control? Build(object? param)
    {
        return param is not (string and var s) ? null : Templates[Path.GetExtension(s)].Build(s);
    }

    public bool Match(object? data)
    {
        return data is string and var s && File.Exists(s);
    }
}