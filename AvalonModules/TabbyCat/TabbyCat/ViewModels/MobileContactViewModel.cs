using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Service.AiServices;
using TuDog.Extensions;
using TuDog.Interfaces.Navigations;
using TuDog.Interfaces.Navigations.Impl;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public sealed partial class MobileContactViewModel(
    INavigationService navigationService,
    IOccupationService occupationService) :AiViewModelBase
{

    [ObservableProperty] private ObservableCollection<OccupationType> occupations = [];

    [ObservableProperty] private OccupationType? selectedOccupation = null;
    
    protected override async Task OnLoaded()
    {
        this.Occupations.Reset( await occupationService.GetAllOccupationsAsync());
    }

    partial void OnSelectedOccupationChanged(OccupationType? value)
    {
        if (value is null)
            return;
        var paraemters = new NavigationParameter();
        paraemters.Add("New",true);
        paraemters.Add("Occupation",value.Occupation);
        navigationService.PushAsync<ChatViewModel>(paraemters);
    }

    [RelayCommand]
    private Task AddNewOccupation()
    {
       return navigationService.PushAsync<MobileNewOccupationViewModel>(null);
    }

    [RelayCommand]
    private Task Search()
    {
       return navigationService.PushAsync<MobileSearchContactViewModel>(null);
    }
}