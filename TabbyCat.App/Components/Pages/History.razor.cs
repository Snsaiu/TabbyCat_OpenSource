using TabbyCat.App.Interfaces;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;
using TabbyCat.Shared.Enums;
using TabbyCat.App.Extensions;
using TabbyCat.App.Models;
using Microsoft.FluentUI.AspNetCore.Components;

using TabbyCat.Repository.Entities.LocalNetShareEntities;
using TabbyCat.Shared.Interfaces;
using TabbyCat.VisualBase.Bases;

namespace TabbyCat.App.Components.Pages;

public partial class History : PageComponentBase
{
    [Inject] private IOpenFolder OpenFolder { get; set; } = null!;

    [Inject] private IDialogService DialogService { get; set; } = null!;

    [Inject] private IOpenFileable OpenFile { get; set; } = null!;

    [Inject] protected ISystemType SystemType { get; set; } = null!;


    [Parameter] public ObservableCollection<SaveItemModel>? Models { get; set; } = [];

    protected override async Task OnPageInitializedAsync(string? url, Dictionary<string, object>? data)
    {
        
        StateManager.ClearNotReadCount();
        
        var list = await SaveDataService.GetAllAsync();

        if (list.Ok)
        {
            list.Data.Reverse();
            Rig(list.Data);
        }
        else
        {
            await DialogService.ShowWarningAsync(list.ErrorMsg ?? String.Empty, "Warning");
        }
    }

    private void Rig(List<SaveDataEntity> sources)
    {
        Models?.Clear();
        foreach (var item in sources)
        {
            var sm = new SaveItemModel
            {
                Title = item.DataType == SendType.Text
                    ? item.Content.Replace(" ", "").Length > 20
                        ? item.Content.Replace(" ", "").Replace("\n", "")[..20] + "..."
                        : item.Content.Replace(" ", "").Replace("\n", "")
                    : Path.GetFileName(item.Content),
                SendType = item.DataType,
                Content = item.Content,
                Guid = item.Guid,
                SourceDeviceName = item.SourceDeviceNickName,
                Time = item.Time.ToString("yyyy-MM-dd HH:mm:ss")
            };

            Models?.Add(sm);
        }
    }

    #region Commands

    private async Task CopyContentCommand(SaveItemModel model)
    {
        await Clipboard.Default.SetTextAsync(model.Content);
        this.ToastService.ShowSuccess("Copied content");
    }

    private async Task OpenFileCommand(SaveItemModel model)
    {
        if (string.IsNullOrEmpty(model.Content))
        {
            throw new NullReferenceException();
        }

        // 判断文件是否存在
        if (File.Exists(model.Content) == false)
        {
            FantasyResultModel.ResultBase<bool> res =
                await SaveDataService.DeleteDataAsync(model.Guid ?? throw new NullReferenceException());
            if (res.Ok)
            {
                Models?.Remove(model);
            }
            else
            {
                ToastService.ShowWarning(res.ErrorMsg ?? string.Empty);
            }
        }

        OpenFile.OpenFile(model.Content);
    }

    private Task OpenFolderCommand(SaveItemModel model)
    {
        if (string.IsNullOrEmpty(model.Content))
        {
            ToastService.ShowWarning("文件路径不存在");
        }

        var p = model.SendType == SendType.File ? Directory.GetParent(model.Content)?.ToString() : model.Content;
        if (string.IsNullOrEmpty(p))
            ToastService.ShowWarning("文件夹父路径不存在");

        OpenFolder.OpenFolder(p!);
        return Task.CompletedTask;
    }

    private async Task DeleteCommand(SaveItemModel model)
    {
        IsBusy = true;
        if (model.SendType == SendType.File)
        {
            if (File.Exists(model.Content))
            {
                File.Delete(model.Content);
            }
        }

        FantasyResultModel.ResultBase<bool> res = await SaveDataService.DeleteDataAsync(model.Guid);
        if (res.Ok)
        {
            Models.Remove(model);
        }
        else
        {
            ToastService.ShowWarning(res.ErrorMsg);
        }

        IsBusy = false;
    }

    #endregion
}