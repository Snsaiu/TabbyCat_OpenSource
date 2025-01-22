using AirTransfer.Interfaces;

using Newtonsoft.Json;

using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;

namespace AirTransfer.Models;

public class SendMetadataMessage : IName, ISendType, IFlag, ISize, ITargetFlag, ICompressable
{
    public SendMetadataMessage(string flag, string targetFlag, string name, long size, bool isCompress)
    {
        SendType = SendType.File;
        Name = name;
        Size = size;
        Flag = flag;
        TargetFlag = targetFlag;
        IsCompress = isCompress;
    }

    public SendMetadataMessage(string flag, string targetFlag, long size)
    {
        SendType = SendType.Text;
        Flag = flag;
        Size = size;
        TargetFlag = targetFlag;
    }

    [JsonConstructor]
    public SendMetadataMessage(string name, string flag, string targetFlag, SendType sendType, long size,
        bool isCompress)
    {
        SendType = sendType;
        Name = name;
        Flag = flag;
        Size = size;
        TargetFlag = targetFlag;
        IsCompress = isCompress;
    }

    public bool IsCompress { get; }
    public string Flag { get; }

    public string Name { get; } = string.Empty;
    public SendType SendType { get; }

    public long Size { get; }
    public string TargetFlag { get; }
}