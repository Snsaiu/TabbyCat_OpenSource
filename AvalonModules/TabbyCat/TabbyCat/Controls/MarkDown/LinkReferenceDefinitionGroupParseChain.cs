using Avalonia.Controls;
using Avalonia.Layout;
using Markdig.Syntax;

namespace TabbyCat.Controls.MarkDown;

public sealed class LinkReferenceDefinitionGroupParseChain : MarkDownParseChainBase<LinkReferenceDefinitionGroup>
{
    protected override Control ParseImpl(LinkReferenceDefinitionGroup block)
    {
        var container = new StackPanel() { Orientation = Orientation.Vertical };
        foreach (var item in block.Links)
        {
            if (string.IsNullOrEmpty(item.Value.Url))
                continue;
            container.Children.Add(new HyperlinkButton() { Content = item.Key, NavigateUri = new(item.Value.Url) });
        }

        return container;
    }
}