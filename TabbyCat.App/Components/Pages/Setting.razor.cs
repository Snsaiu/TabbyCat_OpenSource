using TabbyCat.App.Extensions;
using TabbyCat.App.Interfaces;
using TabbyCat.App.Interfaces.IConfigs;
using TabbyCat.App.Interfaces.Impls.Configs;

using CommunityToolkit.Maui.Storage;

using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;

using TabbyCat.Service.AiServices;
using TabbyCat.Shared.Enums;
using TabbyCat.VisualBase.Bases;

namespace TabbyCat.App.Components.Pages;

public partial class Setting : PageComponentBase
{
    #region Injects

    [Inject] private IUserService UserService { get; set; } = null!;

#if WINDOWS || MACCATALYST
    [Inject] private FileSavePathBase FileSavePath { get; set; } = null!;

    [Inject] private ITopMostService TopMostService { get; set; } = null!;

#endif

    [Inject] private ILanguageService LanguageService { get; set; } = null!;

    [Inject] private IThemeService ThemeService { get; set; } = null!;

   [Inject] private IAiTemplateSettingService AiTemplateSettingService { get; set; } = null!;

#if WINDOWS || MACCATALYST

    [Inject] private IShowCloseDialogService ShowCloseDialogService { get; set; } = null!;

    [Inject] private ICloseAppBehaviorService CloseAppBehaviorService { get; set; } = null!;
#endif

    #endregion

    #region Parameters

    public List<KeyValuePair<string, string>> Languages { get; set; } = [];


    [Parameter] public DesignThemeModes Theme { get; set; }

    [Parameter] public OfficeColor? OfficeColor { get; set; }

    private bool IsClipboardWatch { get; set; } = false;

    private bool isTopMost;

    private string SavePath { get; set; } = String.Empty;

    [Parameter] public string SelectedLanguage { get; set; } = string.Empty;
    #endregion

    #region Private Fields

    private CloseAppBehavior closeAppState;

    private bool closeShowDialog;

    #endregion

    #region Overrides

    protected override Task OnParametersSetAsync()
    {
        Theme = ThemeService.GetDesignTheme();
        OfficeColor = ThemeService.GetThemeColor();

#if WINDOWS || MACCATALYST
        SavePath = FileSavePath.SaveLocation;
        IsClipboardWatch = LoopWatchClipboardService.GetState();

        closeAppState = (CloseAppBehavior)CloseAppBehaviorService.Get<int>();
        closeShowDialog = !ShowCloseDialogService.Get<bool>();
        isTopMost = TopMostService.Get<bool>();
#endif
        return base.OnParametersSetAsync();
    }

    protected override Task OnPageInitializedAsync(string? url, Dictionary<string, object>? data)
    {
        InitLanguages();

        return base.OnPageInitializedAsync(url, data);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
          this.Module = await  this.JsRuntime.InvokeAsync<IJSObjectReference>("import", "./Components/Pages/Setting.razor.js");
          await this.Module.InvokeVoidAsync("reHeight");
        }
    }

    #endregion

    #region Commands

    private Task SaveLanguageCommand(KeyValuePair<string, string> selectedLanguage)
    {
        LanguageService.SetLanguage(selectedLanguage.Value);
        ToastService.ShowInfo(Localizer["SetLanguageInfo"]);
        return Task.CompletedTask;
    }

    private async Task LogoutCommand()
    {
        await UserService.ClearUserAsync();
        //清空所有的任务

        var devices = StateManager.Devices();

        foreach (var device in devices)
            if (device.TransmissionTasks.Any())
                foreach (var task in device.TransmissionTasks)
                    if (task.CancellationTokenSource != null)
                        await task.CancellationTokenSource.CancelAsync();

        // 状态字典全部清空
        StateManager.Cleaer();

        // 清空所有的ai模型数据
        await AiTemplateSettingService.DeleteRangeAsync(x => true);

        NavigationManager.NavigateTo("/");
    }

    #endregion



    #region Private methods

    private void InitLanguages()
    {
        Languages.Add(new("English", "en-US"));
        Languages.Add(new("中文", "zh-hans"));
        Languages.Add(new("日本語", "ja-JP"));
        Languages.Add(new("한국어", "ko-KR"));
        Languages.Add(new("français", "fr-FR"));

        SelectedLanguage = LanguageService.GetLanguage() ?? throw new ArgumentNullException(nameof(LanguageService));

    }

    #endregion


    private void SaveThemeModeCommand(DesignThemeModes designThemeModes)
    {
        Theme = designThemeModes;
        ThemeService.SetDesignTheme(designThemeModes);
    }

    private void SaveColorCommand(OfficeColor? officeColor)
    {
        OfficeColor = officeColor;
        ThemeService.SetThemeColor(officeColor!.Value);
    }


    private async Task SavePathCommand()
    {
#if WINDOWS || MACCATALYST
        var result = await FolderPicker.Default.PickAsync();
        if (result is not { IsSuccessful: true, Folder.Path: var path })
        {
            return;
        }
        ((IChangePathable)FileSavePath).ChangedPath(path);
        SavePath = path;
#endif

    }

    private void ClipboardWatchChangedCommand(bool value)
    {
#if WINDOWS || MACCATALYST
        IsClipboardWatch = value;
        LoopWatchClipboardService.SetState(IsClipboardWatch);
        ToastService.ShowSuccess(Localizer["UpdateClipboardStateMessage"]);
#endif
    }

    private void CloseShowDialogChangedCommand(bool obj)
    {
#if WINDOWS || MACCATALYST
        closeShowDialog = obj;
        ShowCloseDialogService.Set(!closeShowDialog);
#endif
    }

    private void CloseAppBehaviorChangedCommand(CloseAppBehavior obj)
    {
#if WINDOWS || MACCATALYST
        closeAppState = obj;
        CloseAppBehaviorService.Set((int)closeAppState);
#endif
    }

    private void TopMostChangedCommand(bool obj)
    {
#if WINDOWS || MACCATALYST
        this.isTopMost = obj;
        TopMostService.Set(obj);
        this.ToastService.ShowSuccess("重启后生效");
#endif
    }
}