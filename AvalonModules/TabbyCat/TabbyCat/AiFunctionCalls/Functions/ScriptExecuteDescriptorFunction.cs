using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Security.AccessControl;
using System.Threading;
using System.Xml.Linq;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using TabbyCat.AiFunctionCalls.Models.Inputs;

namespace TabbyCat.AiFunctionCalls.Functions;

/// <summary>
/// c# 脚本执行器
/// </summary>
public sealed class ScriptExecuteDescriptorFunction : FunctionCallDescriptorBase<QueryScriptInputModel>
{
    public override string FunctionName { get; } = "ScriptExecute";

    protected override string FunctionDescription()
    {
        return
            "c#脚本执行，你会使用来自Microsoft.CodeAnalysis.CSharp.Scripting 这个库中CSharpScript这个类的RunAsync<string>方法来执行你写的脚本，我已经给你引入了如下的命名空间" +
            "\"System\",\"System.IO\",\"System.Text\",\"System.Text.Json\",\"System.IO.Compression\",\"System.Security.AccessControl\",\"System.Security.Principal\",\"System.Diagnostics\",\"System.Xml\",\"System.Xml.Linq\",\"Newtonsoft.Json\",\"System.Threading\",\"System.Net.Http\","
            + "你执行完成后最后返回一个string结果给我。比如获得路径或者获得什么内容，一定要return 你计算后的数据。并且当你命令执行完之后，就不要重复在执行同一个命令内容了。你可以告诉我你完成了。并且当你认为你要使用ScriptExecute能力，那么你就直接生成相关的json数据给我。不要有多余的话。";
    }

    public override async Task<OutputParameter> QueryDataAsync(QueryScriptInputModel? input,
        CancellationToken cancellationToken = default)
    {
        if (input is null || string.IsNullOrEmpty(input.Script))
            return new() { Ok = false, ErrorMessage = "脚本内容为空" };

        var options = ScriptOptions.Default
            .WithReferences(
                typeof(File).Assembly, // System.IO
                typeof(System.Text.Json.JsonSerializer).Assembly, // System.Text.Json
                typeof(ZipFile).Assembly, // System.IO.Compression
                typeof(FileSystemAccessRule).Assembly, // System.Security.AccessControl
                typeof(XDocument).Assembly, // System.Xml.Linq
                typeof(JsonConvert).Assembly, // Newtonsoft.Json
                typeof(HttpClient).Assembly, // System.Net.Http
                typeof(Process).Assembly
            )
            .WithImports(
                "System",
                "System.IO",
                "System.Text",
                "System.Text.Json",
                "System.IO.Compression",
                "System.Security.AccessControl",
                "System.Security.Principal",
                "System.Xml",
                "System.Xml.Linq",
                "Newtonsoft.Json",
                "System.Threading",
                "System.Diagnostics",
                "System.Net.Http"
            );
        Debug.WriteLine(input.Script);
        var result = await CSharpScript.RunAsync<string>(input.Script, options);
        return new() { Ok = true, Data = "执行完成:" + result.ReturnValue.ToString() };
    }
}