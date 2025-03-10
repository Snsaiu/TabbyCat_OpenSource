using Avalonia;
using Avalonia.Controls;
using Markdig.Syntax;

namespace TabbyCat.Controls.MarkDown;

public sealed class ThematicBreakBlockParseChain : MarkDownParseChainBase<ThematicBreakBlock>
{
    protected override Control ParseImpl(ThematicBreakBlock block)
    {
        return new StackPanel() { Margin = new(0, 2) };
    }
}