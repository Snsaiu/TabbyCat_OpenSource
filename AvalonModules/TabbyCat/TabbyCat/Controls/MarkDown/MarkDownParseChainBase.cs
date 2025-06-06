using Avalonia.Controls;
using Markdig.Syntax;

namespace TabbyCat.Controls.MarkDown;

public abstract class MarkDownParseChainBase
{
    public MarkDownParseChainBase? Next { get; set; }

    public abstract Control? Parse(Block block);
}

public abstract class MarkDownParseChainBase<TBlock> : MarkDownParseChainBase where TBlock : Block
{
    protected abstract Control ParseImpl(TBlock block);

    public override Control? Parse(Block block)
    {
        if (typeof(TBlock) == block.GetType())
            return ParseImpl((TBlock)block);
        if (Next is not null)
            return Next.Parse(block);
        return null;
    }
}