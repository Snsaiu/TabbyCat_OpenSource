using TabbyCat.Shared.Enums;
namespace TabbyCat.App.Models;

public enum CodeWordType
{
    CheckingPort,
    CheckingPortCanNotUse,
    CheckingPortCanUse,
    CancelTransmission
}

public class CodeWordModel : TransmissionTaskModel
{
    public CodeWordModel(string taskId, CodeWordType type, string flag, string targetFlag, int port,
        SendType sendType, DeviceModel deviceModel, CancellationTokenSource? cancellationTokenSource) : base(taskId,
        flag, targetFlag,
        port, sendType, cancellationTokenSource)
    {
        Type = type;
        DeviceModel = deviceModel;
    }


    public static CodeWordModel CreateCodeWord(string taskId, CodeWordType type, string flag, string targetFlag,
        int port,
        SendType sendType, DeviceModel deviceModel, CancellationTokenSource? cancellationTokenSource)
    {
        return new(taskId, type, flag, targetFlag, port, sendType, deviceModel, cancellationTokenSource);
    }

    public CodeWordType Type { get; set; }

    public DeviceModel DeviceModel { get; set; }
}