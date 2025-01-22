using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.FluentUI.AspNetCore.Components;

namespace TabbyCat.Ai.Components
{
    public partial class RenameSession : VisualBase.Bases.VisualPageBase, IDialogContentComponent
    {
        [Required] [Parameter] public string Content { get; set; }
        
        private EditContext _editContext = default!;

        [CascadingParameter] public FluentDialog Dialog { get; set; } = default!;


        protected override void OnInitialized()
        {
            _editContext = new(Content);
        }

        private async Task SaveAsync()
        {
            if (_editContext.Validate())
            {
                await Dialog.CloseAsync(Content);
            }
        }

        private async Task CancelAsync()
        {
            await Dialog.CancelAsync();
        }

    }
}