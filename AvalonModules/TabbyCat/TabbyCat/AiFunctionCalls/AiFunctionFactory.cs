using System.Text;
using TabbyCat.AiFunctionCalls.Functions;

namespace TabbyCat.AiFunctionCalls;

public static class AiFunctionFactory
{
    private static List<IFunctionCallDescriptor> _descriptors =
        [new QueryTimeDescriptorFunction(), new ScriptExecuteDescriptorFunction()];

    public static string Description()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"你是一个智能助手，负责为用户提供信息查询和任务管理服务，并可以执行一些方法。你拥有以下的能力，你可以根据用户的输入选择你认为应该调用的能力，" +
                      $"调用的方式是你输出一段json字符串(注意json输出完成后不要带有任何的其他字符，比如句号或者换行或者空格等，因为我需要解析你的json数据)。{InputParameter.Description()},当你输出json字符串后我会帮你调用相关的方法，最终你会得到一个json返回数据，你要注意当你要调用能力的时候直接发送json数据，不要带有任何其他修饰符，不要以```json```开头，内容直接以{{开头。{OutputParameter.Description()}，" +
                      $"你可以根据返回的数据继续调用其他的能力或者将输出的结果整理告诉我,你一定要记住如果你认为可以告诉我结果了，请不要在发送任何json数据，连{{符号都不要有，因为你发{{我会认为你还需要调用能力");
        sb.AppendLine("以下是你具备的能力:");

        foreach (var item in _descriptors) sb.AppendLine(item.Description());
        return sb.ToString();
    }

    public static async Task<OutputParameter> QueryAsync(string inputJson)
    {
        var inputModel = JsonConvert.DeserializeObject<InputParameter>(inputJson);

        if (inputJson is null)
            throw new NullReferenceException();

        var descriptor = _descriptors.FirstOrDefault(x => x.FunctionName == inputModel!.FunctionName);
        if (descriptor is null)
            return new OutputParameter() { Ok = false, ErrorMessage = "无法获得功能" };
        
        var output = await descriptor.QueryAsync(inputModel?.InputData);
        return output is null
            ? new() { Ok = false, ErrorMessage = "未知错误。" }
            : (OutputParameter)output;
    }
}