namespace TabbyCat.AiFunctionCalls;

public sealed class InputParameter
{
    /// <summary>
    /// 调用的工具名称
    /// </summary>
    public string FunctionName { get; set; } = string.Empty;

    /// <summary>
    /// 输入数据
    /// </summary>
    public object? InputData { get; set; }

    public static string Description()
    {
        return
            $"输入json格式如下:{JsonConvert.SerializeObject(new InputParameter())},FunctionName表示要调用的功能，当 InputData表示要传入的参数，InputData通常是json格式的数据";
    }
}