namespace TabbyCat.AiFunctionCalls;

public sealed class OutputParameter
{
    public bool Ok { get; set; }

    public string Data { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;

    public static string Description()
    {
        return
            $"返回的格式如下:{JsonConvert.SerializeObject(new OutputParameter())},Ok如果为true 表示执行成功，你可以从Data中拿到数据，如果是false，那么表示执行失败,你可以从ErrorMessage获得错误信息。";
    }
}