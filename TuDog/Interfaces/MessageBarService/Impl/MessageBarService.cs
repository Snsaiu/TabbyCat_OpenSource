using TuDog.Enums;
using TuDog.Models;
using TuDog.UIs;

namespace TuDog.Interfaces.MessageBarService.Impl;

public sealed class MessageBarService:IMessageBarService
{
    private InfoBox _infoBox;

    public void ShowSuccess(string message, string title, bool showClose)
    {
        CreateInfoModel(message, title, showClose, MessageState.Success);
    }

    public void ShowError(string message, string title, bool showClose)
    {
        CreateInfoModel(message, title, showClose, MessageState.Error);
    }

    public void ShowWarning(string message, string title, bool showClose)
    {
        CreateInfoModel(message, title, showClose, MessageState.Warning);
    }

    public void RegisterInfoBox(InfoBox infoBox)
    {
        _infoBox=infoBox;
    }

    private void CreateInfoModel(string message, string title, bool showClose, MessageState state)
    {
        if (_infoBox is null)
            throw new NullReferenceException();

        var model = InfoModel.Create(message, title, showClose, state);
        _infoBox.AddNewMessage(model);
    }

}