using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Shared.Enums;
using TuDog.Interfaces.Navigations;
using TuDog.Interfaces.Navigations.Impl;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public sealed partial class MobileSearchContactViewModel : Bases.AiViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IOccupationService _occupationService;
    [ObservableProperty] private string searchText = string.Empty;

    [ObservableProperty] private ObservableCollection<OccupationType> occupations = [];

    [ObservableProperty]
    private OccupationType? selectedOccupation;

    private SourceCache<OccupationType, string> _occupationCache;

    [ObservableProperty]
    private ReadOnlyObservableCollection<OccupationType> _filteredOccupations;

    public MobileSearchContactViewModel( INavigationService navigationService,IOccupationService occupationService)
    {
        _navigationService = navigationService;
        _occupationService = occupationService;
        _occupationCache = new(x=>x.OccupationName);

        _occupationCache.Connect().Filter(this.WhenValueChanged(x => x.SearchText).Select(search =>
                new Func<OccupationType, bool>(o =>
                    string.IsNullOrWhiteSpace(search) ||
                    o.OccupationName.Contains(search, StringComparison.OrdinalIgnoreCase))))
            .Bind(out _filteredOccupations)
            .AsObservableCache();
    }

    [RelayCommand]
    private Task Pop()
    {
       return _navigationService.PopAsync();
    }

    partial void OnSelectedOccupationChanged(OccupationType? value)
    {
        if(value is null)
            return;

        var paraemters = new NavigationParameter();
        paraemters.Add("New",true);
        paraemters.Add("Occupation",value.Occupation);
        _navigationService.PushAsync<ChatViewModel>(paraemters);
    }

    protected override async Task OnLoaded()
    {
        _occupationCache.AddOrUpdate(await _occupationService.GetAllOccupationsAsync());
    }
}