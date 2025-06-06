using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;
using TabbyCat.Shared.Languages;
using TuDog.Interfaces.Navigations;
using TuDog.Interfaces.Navigations.Impl;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public partial class MobileNewOccupationViewModel(INavigationService navigationService) : Bases.AiViewModelBase
{
    [ObservableProperty] private string occupationName = string.Empty;
    [ObservableProperty] private string description = string.Empty;


    [RelayCommand]
    private Task Pop()
    {
        return navigationService.PopAsync();
    }

    [RelayCommand]
    private async Task AddNewOccupation()
    {
        if (string.IsNullOrEmpty(OccupationName))
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.CharacterNameCannotBeEmpty, AppResources.Warning,
                AppResources.Ok);
            return;
        }

        if (string.IsNullOrWhiteSpace(Description))
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.CharacterDescriptionCannotBeEmpty,
                AppResources.Warning, AppResources.Ok);
            return;
        }

        if ((await customAssistantOccupationService.QueryAsync(x =>
                x.Name == OccupationName )).Any())
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.CharacterNameAlreadyExists, AppResources.Warning,
                AppResources.Ok);
            return;
        }

        var entity = new CustomAssistantOccupationEntity()
        {
            Name = OccupationName,
            Description = Description,
        };

        if (await customAssistantOccupationService.AddAsync(entity))
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.AddedSuccessfully, AppResources.Message,
                AppResources.Ok);

            var result = new NavigationParameter();
            result.Add("Occupation", entity);
            await navigationService.PopAsync(result);
        }
        else
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.AddFailed, AppResources.Warning, AppResources.Ok);
        }
    }
}