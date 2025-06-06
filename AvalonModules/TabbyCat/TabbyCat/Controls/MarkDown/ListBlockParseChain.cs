using Avalonia.Controls;
using Markdig.Syntax;

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
            if (block.IsOrdered)
                _ = $"{index}. ";

            if (item is ListItemBlock listItem)
            {
                var control = builder.Parse(listItem);
                container.Children.Add(control ?? new TextBlock() { Text = $"不支持的控件 {listItem.GetType()}" });
            }
        }

        return container;
    }
}