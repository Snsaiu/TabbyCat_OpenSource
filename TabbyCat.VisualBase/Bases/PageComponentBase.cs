using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using TabbyCat.Shared.ConstParameters;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.VisualBase.Bases;

public abstract class PageComponentBase : VisualPageBase,IAsyncDisposable
{
    #region Injects
    [Inject] protected ILoopWatchClipboardService LoopWatchClipboardService { get; set; } = null!;

    #endregion

    #region Fields

    private readonly string fromPage = nameof(fromPage);

    private readonly string data = nameof(data);

    #endregion


    private string jsPathPrefix = $"./_content/";
    
    protected virtual string GetModuleName()=>String.Empty;
    
    protected string GetJsPath()=> $"{jsPathPrefix}{GetModuleName()}";
    
    protected IJSObjectReference? Module { get; set; }
    
    protected sealed override Task OnInitializedAsync()
    {
        StateManager.StateChanged += () => StateChanged();
        return ParseInitPageDataAsync();
    }

    private Task StateChanged()
    {
        return InvokeAsync(StateHasChanged);

    }

    private Task ParseInitPageDataAsync()
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        StateManager.SetState(ConstParams.StateManagerKeys.CurrentUriKey, NavigationManager.ToBaseRelativePath(NavigationManager.Uri));

        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

        Dictionary<string, object>? data = null;
        string? fromUri = null;
        if (query.HasKeys())
        {
            if (query.AllKeys.Any(x => x == fromPage))
            {
                fromUri = query[fromPage];
            }

            if (query.AllKeys.Any(x => x == this.data))
            {
                var dataString = query[this.data];
                data = JsonConvert.DeserializeObject<Dictionary<string, object>>(dataString);
            }
        }

        return OnPageInitializedAsync(fromUri, data);
    }

    protected virtual Task OnPageInitializedAsync(string? url, Dictionary<string, object>? data)
    {
        return Task.CompletedTask;
    }

    protected void NavigateTo(string uri)
    {
        NavigationManager.NavigateTo(uri);
    }

    protected void NavigateTo(string uri, Dictionary<string, object> dataDictionary)
    {
        var currentPage = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var json = JsonConvert.SerializeObject(dataDictionary, new JsonSerializerSettings
        {
            StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
        });
        NavigationManager.NavigateTo($"{uri}?{fromPage}={currentPage.AbsolutePath}&{data}={json}");
    }


    public async ValueTask DisposeAsync()
    {
        if (this.Module is not null)
            await Module.DisposeAsync();
    }
}