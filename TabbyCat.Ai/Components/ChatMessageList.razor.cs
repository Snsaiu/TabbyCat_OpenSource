using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TabbyCat.Ai.Bases;
using TabbyCat.Ai.Models.AiReqRes.AiChatRequests;

namespace TabbyCat.Ai.Components;

public partial class ChatMessageList : AiComponentBase
{
    [Parameter] public required List<MessagesItem> Messages { get; set; }


    [Parameter] public int MarginBottom { get; set; } = 100;

    private IJSObjectReference? module;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            module = await JsRuntime.InvokeAsync<IJSObjectReference>("import",
                $"{this.GetJsPath()}/chatmessagelist.js");
            await module.InvokeVoidAsync("reHeight", MarginBottom);
            await module.InvokeVoidAsync("scrollToBottom");
        }

        await OnScrollAsync();
    }


    [Parameter]
    public bool IsLoading { get; set; } = false;


    private async Task OnScrollAsync()
    {
        if(module is null)
            return;
        
         await module.InvokeVoidAsync("scrollToBottom");
    }

}