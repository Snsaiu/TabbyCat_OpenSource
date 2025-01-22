using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using TabbyCat.Ai.Bases;
using TabbyCat.Ai.Components;
using TabbyCat.Ai.Models;
using TabbyCat.Repository.Entities.AiEntities;

namespace TabbyCat.Ai.Pages;

public partial class AiChat : AiPageComponentBase
{

    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
             this.Module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", $"{this.GetJsPath()}/aichat.js");
            await Module.InvokeVoidAsync("reWidth");
        }
    }
    
    private async Task ShowAiChatMenuCommand()
    {
        if (AiModel == null)
        {
            this.ToastService.ShowWarning("请先配置AI模型");
            return;
        }


        var allSessions = await this.AiChatSessionService.QueryAsync();
        var parameters = new Tuple<List<AiChatSessionEntity>, AiApiModelBase>(allSessions.ToList(), AiModel);


        var dialog = await DialogService.ShowPanelAsync<AiSettingPanel>(parameters,
            new DialogParameters<Tuple<List<AiChatSessionEntity>, AiApiModelBase>>()
        {

            Title = "聊天设置",
            Modal = false,
            TrapFocus = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleDialogResult),

        });
        var dialogResult = await dialog.Result;


    }

    private async Task HandleDialogResult(DialogResult arg)
    {
        if (arg.Cancelled)
        {
            return ;
        }

        if (arg.Data is Tuple<List<AiChatSessionEntity>, AiApiModelBase> data)
        {
            AiModel = data.Item2;
            AiChatSession = data.Item1.FirstOrDefault(x => x.IsDefault);
            if (AiChatSession == null)
            {
                this.ToastService.ShowError("未选择会话");
                goto Reset;
            }
            foreach (var item in data.Item1)
            {
                if (await this.AiChatSessionService.UpdateAsync(item))
                {
                    continue;
                }

                this.ToastService.ShowError("更新会话失败");
                return;
            }

            Reset:
            InitMessageSession();

            await InitChatModelsAsync();

            await InitChatHistoryAsync();
        }

        return;
    }
}