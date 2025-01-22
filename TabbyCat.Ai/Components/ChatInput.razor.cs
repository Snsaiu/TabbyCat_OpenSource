using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using TabbyCat.Ai.Bases;

namespace TabbyCat.Ai.Components;

public partial class ChatInput : AiComponentBase
{
    private string bottomStyle = "";

    private string? text;
    [Parameter] public EventCallback<string> OnSend { get; set; }

    private DotNetObjectReference<ChatInput>? objRef;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Module =
                await JsRuntime.InvokeAsync<IJSObjectReference>("import",
                    $"{this.GetJsPath()}/chatinput.js");
            await Module.InvokeVoidAsync("reWidth");

            this.objRef = DotNetObjectReference.Create(this);
            await Module.InvokeVoidAsync("textAreaListener", objRef);


            if (DeviceInfo.Idiom == DeviceIdiom.Phone)
            {
                await Module.InvokeVoidAsync("setInputHeight");
            }
        }
    }

    [JSInvokable]
    public Task TextSend(string content) => SendAsync(content);

    private  Task SendCommand() => SendAsync(text);

    private async Task SendAsync(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            ToastService.ShowWarning("不能发送空的消息内容");
            return;
        }

        await Module!.InvokeVoidAsync("clearTextArea");
        await OnSend.InvokeAsync(content);
        content = string.Empty;
    }

    private async Task HandleKeyDown(KeyboardEventArgs arg)
    {
        if (arg.Key == "Enter")
        {
            await SendCommand();
        }
    }

    private void OnInput(ChangeEventArgs obj)
    {
        text = obj?.Value?.ToString();
    }
}