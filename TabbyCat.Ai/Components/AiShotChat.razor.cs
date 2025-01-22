using Microsoft.JSInterop;
using TabbyCat.Ai.Bases;

namespace TabbyCat.Ai.Components;
public partial class AiShotChat : AiPageComponentBase
{
    private IJSObjectReference? module;
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            module =
               await JsRuntime.InvokeAsync<IJSObjectReference>("import",
                   $"{this.GetJsPath()}/aishotchat.js");
            await module.InvokeVoidAsync("reHeight");
        }
    }
}