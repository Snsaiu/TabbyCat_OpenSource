using Avalonia.Controls;
using Markdig.Syntax;

namespace TabbyCat.Controls.MarkDown;

public sealed class ListItemBlockParseChain : MarkDownParseChainBase<ListItemBlock>
{
    protected override Control ParseImpl(ListItemBlock block)
    {
        var container = new StackPanel();
        var builder = MarkDownParseBuilder.Build();
        foreach (var item in block)
        {
            var control = builder.Parse(item) ?? new TextBlock() { Text = $"不支持解析:{item.GetType()}" };
            container.Children.Add(control);
        }

        return container;
    }
}