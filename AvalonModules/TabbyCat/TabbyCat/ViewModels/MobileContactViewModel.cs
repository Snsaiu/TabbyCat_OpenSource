using TabbyCat.Models;
using TabbyCat.Service.AiServices;
using TabbyCat.ViewModels.Bases;
using TuDog.Interfaces.Navigations.Impl;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public sealed partial class MobileContactViewModel : ContactViewModelBase
{
    protected override void OnOccupationSelectedChanged(OccupationType? value)
    {
        if (value is null)
            return;
        var paraemters = new NavigationParameter();
        paraemters.Add("New",true);
        paraemters.Add("Occupation",value.Occupation);
        NavigationService.PushAsync<ChatViewModel>(paraemters);
    }

    protected override Task OnAddNewOccupationAsync()
    {
        return NavigationService.PushAsync<MobileNewOccupationViewModel>(null);
    }


    protected override Task OnSearchAsync()
    {
        return NavigationService.PushAsync<MobileSearchContactViewModel>(null);
    }
}