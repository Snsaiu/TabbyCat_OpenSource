using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TabbyCat.Factories;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatResponses;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;
using TabbyCat.Shared;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Extensions;
using TabbyCat.Shared.Interfaces;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels;
using TuDog.Bootstrap;
using TuDog.Extensions;
using TuDog.Interfaces;
using TuDog.Interfaces.IDialogServers;
using TuDog.IocAttribute;

namespace TabbyCat.Components.ViewModels;

[Register]
public partial class ChatPanelSettingViewModel(
    IDialogServer dialogServer,
    IAiChatMessageRecordService aiChatMessageRecordService,
    IAiChatSessionService aiChatSessionService,
    ICustomAssistantOccupationService customAssistantOccupationService,
    IStoreChatRecordService storeChatRecordService) : ParameterViewModelBase, IViewModelResult
{
    [ObservableProperty] private AiApiModelBase aiModel;

    [ObservableProperty]
    private ObservableCollection<OccupationType> occupations = [];

    [ObservableProperty]
    private OccupationType? selectedOccupationType;

    private bool storeChatRecord;


    [ObservableProperty]
    private ObservableCollection<string> models = [];

    [ObservableProperty]
    private string selectedModel = string.Empty;

    private PanelSettingModel settingParameter;

    [ObservableProperty]
    private AiChatSessionEntity? selectedAiChatSessionEntity;


    #region 创建自定义角色字段

    [ObservableProperty]
    private string newOccupationName = string.Empty;

    [ObservableProperty]
    private string newOccupationDescription = string.Empty;

    [ObservableProperty]
    private bool newOccupationIsDefault = false;


    #endregion

    [ObservableProperty]
    private ObservableCollection<AiChatSessionEntity> sessions = new();


    protected override async Task OnLoaded()
    {
        storeChatRecord = storeChatRecordService.Get();

        settingParameter = Parameter as PanelSettingModel;
        AiModel = settingParameter.AiApiModel;
        Sessions.Reset(settingParameter.AllSessions.OrderByDescending(x => x.IsDefault));
        SelectedAiChatSessionEntity = Sessions.FirstOrDefault(x => x.IsDefault);
        if (settingParameter.AiApiModel is IHasModels<string> hasModelsModel)
        {
            Models.Reset(await hasModelsModel.GetModelsAsync());
            SelectedModel = hasModelsModel.SelectedModel;
        }

        await InitOccupationsAsync();
    }

    private async Task InitOccupationsAsync()
    {
        Occupations.Reset( await GetAllOccupationsAsync());
        if (SelectedAiChatSessionEntity.Occupation != AssistantOccupation.Custom)
            SelectedOccupationType =
                Occupations.FirstOrDefault(x => SelectedAiChatSessionEntity.Occupation == x.Occupation);
        else
            SelectedOccupationType = Occupations.FirstOrDefault(x =>
                x.OccupationName == SelectedAiChatSessionEntity.CustomOccupationName);

        if (SelectedOccupationType is null)
            SelectedOccupationType = Occupations.FirstOrDefault();
    }


    [RelayCommand]
    private async Task DeleteCustomOccupation(OccupationType occupationType)
    {
        var deleteConfirm =
            await dialogServer.ShowConfirmDialogAsync(string.Format(AppResources.ConfirmDelete,
                occupationType.OccupationName));
        if(!deleteConfirm)
            return;
        var deleteResult = await customAssistantOccupationService.DeleteAsync(x => occupationType.OccupationName == x.Name);
        if (deleteConfirm)
        {
            await dialogServer.ShowMessageDialogAsync(AppResources.DeleteSuccess);
           await InitOccupationsAsync();
        }
        else
        {
            await dialogServer.ShowMessageDialogAsync(AppResources.DeleteFailed);
        }

    }

    private async Task<IEnumerable<OccupationType>>GetAllOccupationsAsync()
    {
        var customOccupations = await customAssistantOccupationService.QueryAsync();
        var temps = customOccupations.Select(item => new OccupationType(AssistantOccupation.Custom, item.Name)).ToList();

        temps.AddRange(Enum.GetValues<AssistantOccupation>().Where(x=>x!=AssistantOccupation.Custom).Select(item => new OccupationType(item, LocalizationResourceManager.Instance[item.ToString()])));

        return temps;
    }


    [RelayCommand]
    private async Task AddNewOccupation()
    {
        if (string.IsNullOrEmpty(NewOccupationName))
        {
            await dialogServer.ShowMessageDialogAsync(AppResources.CharacterNameCannotBeEmpty);
            return;
        }

        if (string.IsNullOrWhiteSpace(NewOccupationDescription))
        {
            await dialogServer.ShowMessageDialogAsync(AppResources.CharacterDescriptionCannotBeEmpty);
            return;
        }

        if ((await customAssistantOccupationService.QueryAsync(x => x.Name == NewOccupationName)).Any())
        {
            await dialogServer.ShowMessageDialogAsync(AppResources.CharacterNameAlreadyExists);
            return;
        }

        var entity = new CustomAssistantOccupationEntity()
        {
            Name = NewOccupationName,
            Description =  NewOccupationDescription
        };

        if (await customAssistantOccupationService.AddAsync(entity))
        {
            await dialogServer.ShowMessageDialogAsync(AppResources.AddedSuccessfully);
            Occupations.Reset( await GetAllOccupationsAsync());
            if (NewOccupationIsDefault)
                SelectedOccupationType =
                    Occupations.FirstOrDefault(x => x.OccupationName == NewOccupationName);
            NewOccupationName = string.Empty;
            NewOccupationDescription = string.Empty;
            NewOccupationIsDefault = false;
        }
        else
        {
            await dialogServer.ShowMessageDialogAsync(AppResources.AddFailed);
        }
    }

    [RelayCommand]
    private async Task RenameSession()
    {
        if (SelectedAiChatSessionEntity is null) return;

        var name = string.IsNullOrEmpty(SelectedAiChatSessionEntity.CustomTheme)
            ? SelectedAiChatSessionEntity.Theme.Replace("\n","").Replace("\r","").Truncate(20)
            : SelectedAiChatSessionEntity.CustomTheme;

        var result = await dialogServer.ShowInputDialogAsync(name);
        if (result.Ok)
        {
            if (string.IsNullOrEmpty(result.Data))
            {
                await dialogServer.ShowMessageDialogAsync(AppResources.CannotEnterEmptyContent);
                return;
            }

            SelectedAiChatSessionEntity.CustomTheme = result.Data;
            if (await aiChatSessionService.UpdateAsync(SelectedAiChatSessionEntity))
            {
                await dialogServer.ShowMessageDialogAsync(AppResources.RenamedSuccessfully);
            }
            else
            {
                await dialogServer.ShowMessageDialogAsync(AppResources.RenameFailed);
                SelectedAiChatSessionEntity.CustomTheme = name;
            }
        }
        else
        {
            await dialogServer.ShowMessageDialogAsync(AppResources.RenameFailed);
            SelectedAiChatSessionEntity.CustomTheme = name;
        }
    }

    [RelayCommand]
    private async Task DeleteSession()
    {
        if (SelectedAiChatSessionEntity is null)
            return;

        var confirmDelete = await dialogServer.ShowConfirmDialogAsync(AppResources.ConfirmDeleteSelectedSession);
        if (confirmDelete == false)
            return;
        if (await aiChatSessionService.DeleteAsync(x => x.Key == SelectedAiChatSessionEntity.Key) is not null)
        {
            var sessionId = SelectedAiChatSessionEntity.Key;
            Sessions.Remove(SelectedAiChatSessionEntity);
            if (!storeChatRecord) await aiChatMessageRecordService.DeleteRangeAsync(x => x.SessionId == sessionId);

            if (Sessions.Any(x => !x.IsDefault) && Sessions.Count > 0)
            {
                var first = Sessions.First();
                first.IsDefault = true;
                SelectedAiChatSessionEntity = first;
                await aiChatSessionService.UpdateAsync(first);
            }
        }
        else
        {
            await dialogServer.ShowMessageDialogAsync(AppResources.DeleteFailed);
        }
    }

    private void SetOccupationType(Guid sessionKey)
    {
        if (Sessions.FirstOrDefault(x => x.Key == sessionKey) is not null and var find)
        {
            SetOccupation(find);
        }
        else
        {
            if (!Sessions.Any())
                return;
            SetOccupation(Sessions.First());
        }
    }

    private void SetOccupation(AiChatSessionEntity session)
    {
        if (SelectedOccupationType is null)
        {
            session.CustomOccupationName = string.Empty;
            session.Occupation = AssistantOccupation.Common;
        }
        else
        {
            if (SelectedOccupationType.Occupation == AssistantOccupation.Custom)
            {
                session.Occupation = AssistantOccupation.Custom;
                session.CustomOccupationName = SelectedOccupationType.OccupationName;
            }
            else
            {
                session.CustomOccupationName = string.Empty;
                session.Occupation = SelectedOccupationType.Occupation;
            }
        }
    }

    public object Confirm()
    {
        if (SelectedAiChatSessionEntity is not null and var select)
        {
            var selectedId = select.Key;
            foreach (var item in Sessions) item.IsDefault = false;

            Sessions.First(x => x.Key == selectedId).IsDefault = true;
            SetOccupationType(selectedId);
        }
        else
        {
            if (Sessions.Any())
            {
                Sessions.First().IsDefault = true;
                SetOccupation(Sessions.First());
            }
        }

        if (AiModel is IHasModels<string> model) model.SelectedModel = selectedModel;

        var result = new Tuple<IEnumerable<AiChatSessionEntity>, AiApiModelBase>(Sessions, AiModel);
        return result;

    }

    public object Cancel()
    {
        return null;
    }
}