using CommunityToolkit.Mvvm.ComponentModel;
using TabbyCat.IServices;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels.Bases;
using TuDog.Bootstrap;
using TuDog.Enums;
using TuDog.Interfaces.IDialogServers;
using TuDog.IocAttribute;

namespace TabbyCat.Components.ViewModels;

[Register]
public sealed partial class AddNewOccupationViewModel(
    ICustomAssistantOccupationService customAssistantOccupationService)
    : DialogViewModelBaseAsync<bool, CustomAssistantOccupationEntity>
{
    [ObservableProperty] private string _name;

    [ObservableProperty] private string _description;

    public override async Task<bool> CanConfirmAsync()
    {
        if (string.IsNullOrEmpty(Name))
        {
            ErrorMessageAction?.Invoke(AppResources.MustInputContact, AppResources.Warning,
                MessageState.Error);
            return false;
        }

        if (string.IsNullOrEmpty(Description))
        {
            ErrorMessageAction?.Invoke(AppResources.MustInputContactDescription, AppResources.Warning,
                MessageState.Error);
            return false;
        }

        if ((await customAssistantOccupationService.QueryAsync(x => x.Name == Name)).Any())
        {
            ErrorMessageAction?.Invoke(string.Format(AppResources.ExistContactName, Name), AppResources.Warning,
                MessageState.Error);
            return false;
        }
        return true;
    }

    public override async Task<CustomAssistantOccupationEntity> ConfirmAsync()
    {
        var entity = new CustomAssistantOccupationEntity
        {
            Name = Name,
            Description = Description,
        };
        await customAssistantOccupationService.AddAsync(entity);
        return entity;
    }

    public override Task<CustomAssistantOccupationEntity> CancelAsync()
    {
        return Task.FromResult<CustomAssistantOccupationEntity>(null);
    }
}