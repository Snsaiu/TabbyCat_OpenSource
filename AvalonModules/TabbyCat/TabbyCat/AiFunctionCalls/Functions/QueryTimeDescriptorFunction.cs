using System.Threading;
using TabbyCat.AiFunctionCalls.Models.Inputs;

namespace TabbyCat.AiFunctionCalls.Functions;

public sealed class QueryTimeDescriptorFunction : FunctionCallDescriptorBase<QueryTimeInputModel>
{
    public override string FunctionName { get; } = "QueryTime";

    protected override string FunctionDescription()
    {
        return $"获得指定地点的当前日期或者时间。注意只要用户询问时间或日期，你都应该使用QueryTime这个能力！不要使用上下文的记忆！！！如果没有指定地点，那么参数的QueryLocation为空。";
    }

    public override Task<OutputParameter> QueryDataAsync(QueryTimeInputModel? input,
        CancellationToken cancellationToken = default)
    {
        if (input is null || string.IsNullOrEmpty(input.QueryLocation))
            return Task.FromResult(new OutputParameter() { Data = DateTime.Now.ToString("f"), Ok = true });
        else
            //todo:查询api接口
            return Task.FromResult(new OutputParameter() { Ok = false, ErrorMessage = "方法未实现。" });
    }
}