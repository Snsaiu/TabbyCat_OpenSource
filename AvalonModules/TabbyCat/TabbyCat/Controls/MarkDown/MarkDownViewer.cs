using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Markdig;

namespace TabbyCat.Controls.MarkDown;

/// <summary>
/// markdown
/// </summary>
public class MarkDownViewer : ContentControl
{
    public static readonly StyledProperty<string> MarkDownProperty = AvaloniaProperty.Register<MarkDownViewer, string>(
        nameof(MarkDown), defaultBindingMode: BindingMode.OneWay);

    private StackPanel _contentPanel = new()
        { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Stretch };

    public string MarkDown
    {
        get => GetValue(MarkDownProperty);
        set => SetValue(MarkDownProperty, value);
    }

    public MarkDownViewer()
    {
        Content = _contentPanel;
        this.GetObservable(MarkDownProperty).Subscribe(UpdateMarkdown);
    }

    private void UpdateMarkdown(string markdown)
    {
        _contentPanel.Children.Clear();
        if (string.IsNullOrEmpty(markdown))
            return;
        var pipline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        var document = Markdig.Markdown.Parse(markdown, pipline);

        var paragraphBlockParseChain = MarkDownParseBuilder.Build();

        foreach (var block in document)
        {
            var control = paragraphBlockParseChain.Parse(block);
            if (control is null)
            {
                _contentPanel.Children.Add(new TextBlock()
                    { Foreground = Brushes.Red, Text = $"无法解析:”{block.GetType()}“类型数据。" });
                continue;
            }

            _contentPanel.Children.Add(control);
        }
    }
}