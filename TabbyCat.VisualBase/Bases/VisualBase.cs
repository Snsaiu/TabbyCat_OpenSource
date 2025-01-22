using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using TabbyCat.Service.LocalNetShareServices;
using TabbyCat.Shared;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.VisualBase.Bases;

public abstract class VisualPageBase : FluentComponentBase
{
    [Inject] protected NavigationManager NavigationManager { get; set; } = null!;

    [Inject] protected IJSRuntime JsRuntime { get; set; } = null!;

    [Inject] protected IStateManager StateManager { get; set; } = null!;

    [Inject] protected IToastService ToastService { get; set; } = null!;

    [Inject] protected ISaveDataService SaveDataService { get; set; } = null!;

    [Inject] protected IDialogService DialogService { get; set; } = null!;

    protected LocalizationResourceManager Localizer => LocalizationResourceManager.Instance;
    [Inject] protected ISystemType SystemType { get; set; } = null!;

    public bool IsBusy { get; set; }
}