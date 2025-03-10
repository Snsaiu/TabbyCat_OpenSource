using System.Text;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace TabbyCat.Controls.MarkDown;

public sealed class ListBlockParseChain : MarkDownParseChainBase<ListBlock>
{
    protected override Control ParseImpl(ListBlock block)
    {
        var index = 0;
        var container = new StackPanel();


        var builder = MarkDownParseBuilder.Build();

        foreach (var item in block)
        {
            index++;
            var header = "• ";
            if (block.IsOrdered)
                header = $"{index}. ";

            if (item is ListItemBlock listItem)
            {
                var control = builder.Parse(listItem);
                container.Children.Add(control ?? new TextBlock() { Text = $"不支持的控件 {listItem.GetType()}" });
            }
        }

        return container;
    }

    private string GetListItemText(ListItemBlock listItemBlock)
    {
        var result = "";

        foreach (var subBlock in listItemBlock)
            if (subBlock is ParagraphBlock paragraph)
                result += GetParagraphText(paragraph) + " ";

        return result.Trim();
    }

    private string GetParagraphText(ParagraphBlock paragraph)
    {
        var text = "";
        foreach (var inline in paragraph.Inline)
            if (inline is LiteralInline literal)
            {
                text += literal.Content.Text;
            }
            else if (inline is EmphasisInline emphasis)
            {
                foreach (var subInline in emphasis)
                    if (subInline is LiteralInline subLiteral)
                        text += subLiteral.Content.Text;
            }
            else if (inline is LinkInline link)
            {
                text += $"[{link.FirstChild.ToString()}]({link.Url})";
            }

        return text;
    }
}