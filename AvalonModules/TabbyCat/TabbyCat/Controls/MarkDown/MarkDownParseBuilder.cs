using Avalonia.Controls;
using Markdig.Syntax;

namespace TabbyCat.Controls.MarkDown;

public static class MarkDownParseBuilder
{
    public static MarkDownParseChainBase Build()
    {
        ParagraphBlockParseChain paragraphBlockParseChain = new();
        FencedCodeBlockParseChain fencedCodeBlockParseChain = new();
        HeadingBlockParseChain headingBlockParseChain = new();
        ListBlockParseChain listBlockParseChain = new();
        LinkReferenceDefinitionGroupParseChain linkReferenceDefinitionGroupParseChain = new();
        ListItemBlockParseChain listItemBlockParseChain = new();
        ThematicBreakBlockParseChain thematicBreakBlockParseChain = new();

        paragraphBlockParseChain.Next = fencedCodeBlockParseChain;
        fencedCodeBlockParseChain.Next = headingBlockParseChain;
        headingBlockParseChain.Next = listBlockParseChain;
        listBlockParseChain.Next = linkReferenceDefinitionGroupParseChain;
        linkReferenceDefinitionGroupParseChain.Next = listItemBlockParseChain;
        listItemBlockParseChain.Next = thematicBreakBlockParseChain;

        return paragraphBlockParseChain;
    }
}