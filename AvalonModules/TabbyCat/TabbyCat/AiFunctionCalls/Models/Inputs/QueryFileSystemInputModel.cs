namespace TabbyCat.AiFunctionCalls.Models.Inputs;

public sealed class QueryFileSystemInputModel : ParameterModelBase
{
    public string FolderPath { get; set; } = string.Empty;

    public string? FileName { get; set; }

    public string? Extension { get; set; }

    public string? WriteContent { get; set; }

    public int Operation { get; set; }

    protected override string ParameterDescription()
    {
        return
            $"{nameof(FolderPath)}:工作路径，表示创建/删除/查看文件的路径；{nameof(FileName)}:需要操控的文件名称，不带后缀。{nameof(Extension)}:文件后缀名，形如.txt；" +
            $"{nameof(WriteContent)}:要写入的内容，如果是仅仅查看或者删除文件，那么这个字段可以为空；{nameof(Operation)}:操作类型，0代表写入文件，1代表删除文件，2代表查看文件";
    }
}