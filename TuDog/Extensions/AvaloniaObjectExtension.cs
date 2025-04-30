using Avalonia;
using Avalonia.Xaml.Interactions.Core;
using Avalonia.Xaml.Interactivity;
using TuDog.Bootstrap;

namespace TuDog.Extensions;

internal static class AvaloniaObjectExtension
{
    public static void AttachLoadedBehavior(this AvaloniaObject view, ITuDogViewModel vm)
    {
        var eventTriggerBehavior = new EventTriggerBehavior()
        {
            EventName = "Loaded"
        };
        var invokeCommandAction = new InvokeCommandAction()
        {
            Command = vm.LoadedCommand
        };
        eventTriggerBehavior.Actions.Add(invokeCommandAction);
        Interaction.GetBehaviors(view).Add(eventTriggerBehavior);
    }

    public static void AttachUnLoadedBehavior(this AvaloniaObject view, ITuDogViewModel vm)
    {
        var eventTriggerBehavior = new EventTriggerBehavior()
        {
            EventName = "Unloaded"
        };
        var invokeCommandAction = new InvokeCommandAction()
        {
            Command = vm.UnLoadedCommand
        };
        eventTriggerBehavior.Actions.Add(invokeCommandAction);
        Interaction.GetBehaviors(view).Add(eventTriggerBehavior);
    }
}