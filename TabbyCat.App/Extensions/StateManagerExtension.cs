using System.Collections.ObjectModel;

using TabbyCat.App.Interfaces;
using TabbyCat.App.Models;

using TabbyCat.Shared.ConstParameters;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.App.Extensions;

public static class StateManagerExtension
{
    private static void CreateInformationModelContainer(IStateManager manager)
    {
        if (!manager.ExistKey(ConstParams.StateManagerKeys.InformationModelKey))
            manager.SetState<InformationModel>(ConstParams.StateManagerKeys.InformationModelKey, null);
    }

    public static void CreateIsWorkingBusyContainer(IStateManager manager)
    {
        if(!manager.ExistKey(ConstParams.StateManagerKeys.IsWorkBusyKey))
            manager.SetState(ConstParams.StateManagerKeys.IsWorkBusyKey, false);
    }

    public static void CreateNotReadCountContainer(IStateManager manager)
    {
        if (!manager.ExistKey(ConstParams.StateManagerKeys.NotReadCountKey))
            manager.SetState(ConstParams.StateManagerKeys.NotReadCountKey, 0);
    }

    public static void AppendNotReadCount(this IStateManager manager)
    {
        CreateNotReadCountContainer(manager);
        var currentValue = manager.GetState<int>(ConstParams.StateManagerKeys.NotReadCountKey);
        manager.SetState<int>(ConstParams.StateManagerKeys.NotReadCountKey, currentValue + 1);
    }

    public static void ClearNotReadCount(this IStateManager manager)
    {
        CreateNotReadCountContainer(manager);
        manager.SetState<int>(ConstParams.StateManagerKeys.NotReadCountKey, 0);
    }

    public static int GetNotReadCount(this IStateManager manager)
    {
        CreateNotReadCountContainer(manager);
        return manager.GetState<int>(ConstParams.StateManagerKeys.NotReadCountKey);
    }

    public static void SetIsWorkingBusyState(this IStateManager manager, bool isbusy)
    {
        CreateIsWorkingBusyContainer(manager);
        manager.SetState(ConstParams.StateManagerKeys.IsWorkBusyKey, isbusy);
    }

    public static bool GetIsWorkingBusyState(this IStateManager manager)
    {
        CreateIsWorkingBusyContainer(manager);
        return manager.GetState<bool>(ConstParams.StateManagerKeys.IsWorkBusyKey);
    }

    #region DiscoveryModels

    private static void CreateDeviceContainer(IStateManager manager)
    {
        if (!manager.ExistKey(ConstParams.StateManagerKeys.DevicesKey))
            manager.SetState(ConstParams.StateManagerKeys.DevicesKey,
                new ObservableCollection<DiscoveredDeviceModel>());
    }
    public static void AddDiscoveryModel(this IStateManager manager, DiscoveredDeviceModel device)
    {
        CreateDeviceContainer(manager);
        manager.GetState<ObservableCollection<DiscoveredDeviceModel>>(ConstParams.StateManagerKeys.DevicesKey)!
            .Add(device);
    }

    public static void RemoveDiscoveryModel(this IStateManager manager, DiscoveredDeviceModel device)
    {
        CreateDeviceContainer(manager);
        var devices =
            manager.GetState<ObservableCollection<DiscoveredDeviceModel>>(ConstParams.StateManagerKeys.DevicesKey);
        if (devices!.Any(x => x.Flag == device.Flag)) devices.Remove(device);
    }

    public static bool ExistDiscoveryModel(this IStateManager manager, string? flag)
    {
        CreateDeviceContainer(manager);
        var devices =
            manager.GetState<ObservableCollection<DiscoveredDeviceModel>>(ConstParams.StateManagerKeys.DevicesKey);
        return devices!.Any(x => x.Flag == flag);
    }

    public static DiscoveredDeviceModel? FindDiscoveredDeviceModel(this IStateManager manager, string? flag)
    {
        CreateDeviceContainer(manager);
        return manager.GetState<ObservableCollection<DiscoveredDeviceModel>>(ConstParams.StateManagerKeys.DevicesKey)!
            .FirstOrDefault(x => x.Flag == flag);
    }

    public static void ClearDiscoveryModel(this IStateManager manager)
    {

        CreateDeviceContainer(manager);
        manager.GetState<ObservableCollection<DiscoveredDeviceModel>>(ConstParams.StateManagerKeys.DevicesKey)!.Clear();
    }

    public static IReadOnlyList<DiscoveredDeviceModel> Devices(this IStateManager manager)
    {
        CreateDeviceContainer(manager);
        return manager.GetState<ObservableCollection<DiscoveredDeviceModel>>(ConstParams.StateManagerKeys.DevicesKey)!;
    }

    public static ObservableCollection<DiscoveredDeviceModel> ObservableDevices(this IStateManager manager)
    {
        CreateDeviceContainer(manager);
        return manager.GetState<ObservableCollection<DiscoveredDeviceModel>>(ConstParams.StateManagerKeys.DevicesKey)!;
    }

    #endregion

    public static InformationModel? GetInformationModel(this IStateManager manager)
    {
        CreateInformationModelContainer(manager);
        return manager.GetState<InformationModel>(ConstParams.StateManagerKeys.InformationModelKey);
    }

    public static void SetInformationModel(this IStateManager manager, InformationModel? informationModel)
    {
        CreateInformationModelContainer(manager);
        manager.SetState(ConstParams.StateManagerKeys.InformationModelKey, informationModel);
    }


}