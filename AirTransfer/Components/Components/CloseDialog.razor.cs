using AirTransfer.Interfaces.IConfigs;
using Microsoft.AspNetCore.Components;

using TabbyCat.VisualBase.Bases;

namespace AirTransfer.Components.Components;

public partial class CloseDialog : DialogBase
{
#if MACCATALYST || WINDOWS
    [Inject] private IShowCloseDialogService ShowCloseDialogService { get; set; } = null!;

#endif
    private bool isChecked = false;

    protected override void OnInitialized()
    {
#if MACCATALYST || WINDOWS
        isChecked = ShowCloseDialogService.Get<bool>();
#endif
    }

    private void CheckedChanged(bool? value)
    {
#if MACCATALYST || WINDOWS
         isChecked = value!.Value;
        ShowCloseDialogService.Set(value!.Value);
#endif
    }

}