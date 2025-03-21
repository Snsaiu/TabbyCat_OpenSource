namespace TabbyCat.AiFunctionCalls.Models.Inputs;

public sealed class QueryScriptInputModel : ParameterModelBase
{
    public string Script { get; set; } = string.Empty;

    protected override string ParameterDescription()
    {
        return
            "c#脚本执行内容";
    }
}