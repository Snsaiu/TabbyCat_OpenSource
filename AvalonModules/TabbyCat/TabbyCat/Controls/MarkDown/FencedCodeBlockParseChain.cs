using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using Markdig.Syntax;
using TabbyCat.Extensions;

namespace TabbyCat.Controls.MarkDown;

/// <summary>
/// 代码块
/// </summary>
public sealed class FencedCodeBlockParseChain : MarkDownParseChainBase<FencedCodeBlock>
{
    protected override Control ParseImpl(FencedCodeBlock block)
    {
        var language = block.Info ?? string.Empty;
        var code = block.Lines.ToString().Trim();
        var panel = new StackPanel()
            { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Stretch };
        // 添加button
        var button = new Button() { HorizontalAlignment = HorizontalAlignment.Right };
        button.Classes.Add("icon");
        button.Click += async (_, _) =>
        {
            var clipboard = App.TopLevel.Clipboard;
            var dataObject = new DataObject();
            dataObject.Set(DataFormats.Text, code);
            await clipboard!.SetTextAsync(code);
        };
        IconAttach.SetIcon(button, IconFontProvider.copy);
        panel.Children.Add(button);

        var editor = new TextEditor
        {
            Text = code,
            IsReadOnly = true,
            ShowLineNumbers = true,
            VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
            // SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#")
        };
        if (!string.IsNullOrEmpty(language))
        {
            if (language == "csharp")
                language = "C#";
            editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition(language);
        }

        panel.Children.Add(editor);
        return panel;
    }
}