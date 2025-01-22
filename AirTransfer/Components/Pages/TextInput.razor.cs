using Microsoft.AspNetCore.Components;

using TabbyCat.VisualBase.Bases;

namespace AirTransfer.Components.Pages;

public partial class TextInput : PageComponentBase
{

    private string? _text;


    private void SubmitCommand()
    {
        if (string.IsNullOrEmpty(_text))
        {
            ToastService.ShowError(Localizer["InputTextPlaceholder"]);
            return;
        }

        NavigateTo("/home", new Dictionary<string, object> { { "text", _text } });
    }

    private void ReturnCommand()
    {
        NavigateTo("/home");
    }
}