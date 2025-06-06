using System.Text;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace TabbyCat.Controls.MarkDown;

public sealed class ParagraphBlockParseChain : MarkDownParseChainBase<ParagraphBlock>
{
    protected override Control ParseImpl(ParagraphBlock block)
    {
        var stackPanel = new WrapPanel() { Orientation = Orientation.Horizontal };
        var sb = new StringBuilder();

        if (block.Inline is null)
            return stackPanel;
        
        foreach (var child in block.Inline)
            if (child is EmphasisInline emphasis)
            {
                sb.Append(ParseEmphasisInline(emphasis));
            }
            else if (child is LinkInline link)
            {
                if (sb.Length > 0)
                {
                    stackPanel.Children.Add(new SelectableTextBlock()
                        { Text = sb.ToString(), TextWrapping = TextWrapping.Wrap });
                    sb.Clear();
                }

                if (link.FirstOrDefault() is LiteralInline literal)
                    stackPanel.Children.Add(new HyperlinkButton()
                        { Content = literal.Content, NavigateUri = new(link.Url??string.Empty) });
                else
                    throw new NotImplementedException();
            }
            else if (child is CodeInline code)
            {
                sb.Append($"\"{code.Content}\"");
            }
            else
            {
                if (sb.Length > 0)
                {
                    stackPanel.Children.Add(new SelectableTextBlock()
                        { Text = sb.ToString(), TextWrapping = TextWrapping.Wrap });
                    sb.Clear();
                }

                stackPanel.Children.Add(new SelectableTextBlock()
                    { Text = child.ToString(), TextWrapping = TextWrapping.Wrap });
            }

        return stackPanel;
    }


    private string ParseEmphasisInline(EmphasisInline block)
    {
        var sb = new StringBuilder();
        foreach (var subInline in block)
            if (subInline is LiteralInline literal)
                sb.Append(literal.Content.ToString());
            else if (subInline is CodeInline code)
                sb.Append($"\"{code.Content}\"");
            else
                throw new NotImplementedException();
        return sb.ToString();
    }
}