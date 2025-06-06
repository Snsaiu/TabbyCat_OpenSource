using Avalonia.Controls;
using Avalonia.Media;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace TabbyCat.Controls.MarkDown;

public sealed class HeadingBlockParseChain : MarkDownParseChainBase<HeadingBlock>
{
    protected override Control ParseImpl(HeadingBlock block)
    {
        var fontSize = GetFontSizeByHeadingLevel(block.Level);

        if (block.Inline is null)
            return new SelectableTextBlock();
        
        var text = string.Empty;
        foreach (var line in block.Inline)
            if (line is LiteralInline literal)
                text += literal.Content.ToString();
            else if (line is EmphasisInline emphasis)
                text += emphasis.First().ToString();
            else if (line is CodeInline code)
                text += $"\"{code.Content}\"";
            else
                throw new NotImplementedException();

        return new SelectableTextBlock()
            { Text = text ?? string.Empty, FontSize = fontSize, TextWrapping = TextWrapping.Wrap };
    }

    private double GetFontSizeByHeadingLevel(int level)
    {
        return level switch
        {
            1 => 32, // h1
            2 => 24, // h2
            3 => 20, // h3
            4 => 18, // h4
            5 => 16, // h5
            6 => 14, // h6
            _ => 14 // 默认
        };
    }
}