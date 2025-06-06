namespace TabbyCat.AiFunctionCalls.Models.Inputs;

public sealed class QueryTimeInputModel : ParameterModelBase
{
    public string QueryLocation { get; set; } = string.Empty;

    protected override string ParameterDescription()
    {
        return "QueryLocation:要查哪个地方的时间,比如北京,上海。这个参数可以为空";
    }
}