using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using TabbyCat.Components.Views;
using TabbyCat.Controls;
using TabbyCat.IServices.LocalConfigs;
using TuDog.Bootstrap;
using TuDog.IocAttribute;

namespace TabbyCat.Components.ViewModels;

[Register]
public sealed partial class DrawMaskViewModel(ICacheFolderService cacheFolderService):DialogViewModelBaseAsync<string,string>
{
    
    public ImageDrawControl View { get; set; }
    protected override Task OnLoaded()
    {
         base.OnLoaded();
         if(!string.IsNullOrEmpty(Parameter))
             BackgroundImage = Parameter;
         return Task.CompletedTask;
    }

    [ObservableProperty]
    private string _backgroundImage = string.Empty;

    public override async Task<string> ConfirmAsync()
    {
        var fileName = Path.Combine(cacheFolderService.Get(), $"{Guid.NewGuid():N}.png");

        await View.SaveWithWhiteBackgroundAsync(fileName);
        return fileName;
    }

    public override Task<string> CancelAsync()
    {
        return Task.FromResult(string.Empty);
    }
}