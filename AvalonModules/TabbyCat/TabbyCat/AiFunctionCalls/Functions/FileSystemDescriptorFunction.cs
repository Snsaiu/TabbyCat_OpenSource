using System.Threading;
using TabbyCat.AiFunctionCalls.Models.Inputs;

namespace TabbyCat.AiFunctionCalls.Functions;

public class FileSystemDescriptorFunction : FunctionCallDescriptorBase<QueryFileSystemInputModel>
{
    public override string FunctionName { get; } = "QueryFileSystem";

    protected override string FunctionDescription()
    {
        return "用于操作用户磁盘文件，可以对指定路径写入指定的文件或者删除指定的文件。";
    }

    public override Task<OutputParameter> QueryDataAsync(QueryFileSystemInputModel? input,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new OutputParameter());
    }
}