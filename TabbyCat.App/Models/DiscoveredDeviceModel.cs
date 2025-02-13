namespace TabbyCat.App.Models;

public enum WorkState
{
    None,
    Sending,
    Downloading,
    Waiting,
}

/// <summary>
///     设备发现的模型
/// </summary>
public class DiscoveredDeviceModel : DeviceModel
{
    public bool IsChecked { get; set; }

    public double Progress { get; set; }

    public WorkState WorkState { get; set; }

    public List<TransmissionTaskModel> TransmissionTasks { get; } = [];

    public bool TryGetTransmissionTask(string taskId, out TransmissionTaskModel? task)
    {
        task = null;
        if (TransmissionTasks.Any(x => x.TaskGuid == taskId))
            task = TransmissionTasks.FirstOrDefault(x => x.TaskGuid == taskId);
        return task != null;
    }

    public void RemoveTransmissionTask(string taskId)
    {
        TransmissionTasks.Remove(TransmissionTasks.FirstOrDefault(x => x.TaskGuid == taskId));
    }

    public DiscoveredDeviceModel()
    {
    }

    public DiscoveredDeviceModel(DeviceModel device)
    {
        NickName = device.NickName;
        DeviceType = device.DeviceType;
        SystemType = device.SystemType;
        Flag = device.Flag;
        DeviceName = device.DeviceName;
    }

    public static implicit operator DiscoveredDeviceModel(JoinMessageModel model)
    {
        return new DiscoveredDeviceModel
        {
            NickName = model.DeviceName,
            Flag = model.Flag,
            SystemType = model.SystemType,
            DeviceType = model.Device.ToString()
        };
    }
}