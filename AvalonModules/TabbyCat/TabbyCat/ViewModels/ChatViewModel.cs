using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TabbyCat.Components.ViewModels;
using TabbyCat.Factories;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests;
using TabbyCat.Models.AiReqRes.AiChatResponses;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;
using TabbyCat.Shared;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Languages;
using TuDog.Interfaces;
using TuDog.Interfaces.IDialogServers;
using TuDog.Interfaces.RegionManagers;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public partial class ChatViewModel(
    IRegionManager regionManager,
    IDialogServer dialogServer,
    IAiChatSessionService chatSessionService,
    IAiTemplateSettingService aiTemplateSettingService,
    ICustomAssistantOccupationService customAssistantOccupationService,
    IUseMarkdownService useMarkdownService,
    IAiChatMessageRecordService aiChatMessageRecordService)
    : AiViewModelBase(aiTemplateSettingService, aiChatMessageRecordService)
{
    [ObservableProperty] private bool showPanel = false;

    private AiApiModelBase? aiApiModelBase;

    [ObservableProperty] private AiChatSessionEntity aiChatSession;

    private MessageSessionBase? messageSession;

    [ObservableProperty] private bool isBusy = false;

    private CancellationTokenSource? cancelTokenSource;
    public Action ChatItemChanged { get; set; }


    private IViewModelResult? panelSettingResult;

    [ObservableProperty]
    private string inputTextContent = string.Empty;

    [ObservableProperty]
    private ObservableCollection<MessagesItem> chatModels = new();

    private bool showMarkDownState;

    protected override Task OnLoaded()
    {
        showMarkDownState = useMarkdownService.Get();
        return InitializedAsync();
    }

    private async Task InitializedAsync()
    {
        var defaultModels = await aiTemplateSettingService.QueryAsync(x => x.IsDefault);
        if (!defaultModels.Any())
        {
            await this.DialogServer.ShowMessageDialogAsync(AppResources.PleaseSelectAIModelFirst, AppResources.Warning);
            return;
        }

        aiApiModelBase = defaultModels.First().Provider == AiModelType.Custom
            ? await AiTemplateFactory.GetTemplateAsync(defaultModels.First().ModelName, defaultModels)
            : await AiTemplateFactory.GetTemplateAsync(defaultModels.First().Provider, defaultModels);

        await InitAiChatSessionAsync();

        InitMessageSession();

        await InitChatModelsAsync();

        await InitChatHistoryAsync();
    }


    private void InitMessageSession()
    {
        if (aiApiModelBase is null)
            return;
        messageSession = AiRequestFactory.CreateMessageSession(aiApiModelBase);
    }

    [RelayCommand]
    private Task? CancelRequestChat()
    {
        return cancelTokenSource?.CancelAsync();
    }

    private async Task InitChatHistoryAsync()
    {
        var histories = await aiChatMessageRecordService.QueryAsync(x => x.SessionId == AiChatSession.Key);
        if (!histories.Any()) return;

        var orderbyTimes = histories.OrderBy(x => x.CreateTime);
        foreach (var history in orderbyTimes)
            ChatModels.Add(new()
            {
                Key = history.Key, Content = history.Content, Role = history.Role, IsFavourite = history.IsFavourite,
                ShowMarkdownMode = showMarkDownState
            });

        if (aiApiModelBase is null)
            return;
        if (!aiApiModelBase.ContextCountLimit)
        {
            foreach (var chatmodel in ChatModels)
                messageSession?.Messages.Add(new()
                {
                    Key = chatmodel.Key, Content = chatmodel.Content, Role = chatmodel.Role,
                    IsFavourite = chatmodel.IsFavourite, ShowMarkdownMode = showMarkDownState
                });
        }
        else
        {
            var tasks = ChatModels.TakeLast(aiApiModelBase.ContextCount);
            foreach (var chatmodel in tasks)
                messageSession?.Messages.Add(new()
                {
                    Key = chatmodel.Key, Content = chatmodel.Content, Role = chatmodel.Role,
                    IsFavourite = chatmodel.IsFavourite, ShowMarkdownMode = showMarkDownState
                });
        }
    }

    private async Task InitChatModelsAsync(bool addDefaultMessage = true)
    {
        ChatModels.Clear();
        messageSession?.Messages.Clear();
        if (addDefaultMessage)
        {
            if (AiChatSession.Occupation == AssistantOccupation.Custom)
            {
                var occupation =
                    await customAssistantOccupationService.QueryAsync(x =>
                        x.Name == AiChatSession.CustomOccupationName);
                messageSession?.Messages.Add(new MessagesItem
                    { Content = occupation.FirstOrDefault()?.Description ?? string.Empty, Role = Role.System });
            }
            else
            {
                var discritpion = LocalizationResourceManager.Instance[$"{AiChatSession.Occupation.ToString()}Prompt"];
                messageSession?.Messages.Add(new MessagesItem { Content = discritpion, Role = Role.System });
            }
        }
    }

    private async Task InitAiChatSessionAsync()
    {
        var finds = await chatSessionService.QueryAsync(x => x.IsDefault);
        if (finds.Any()) AiChatSession = finds.First();
        else
        {
            AiChatSession = AiChatSessionEntity.CreateDefault();
            await chatSessionService.AddAsync(AiChatSession);
        }
    }

    [RelayCommand]
    private async Task Send()
    {
        if (string.IsNullOrEmpty(InputTextContent))
        {
            await dialogServer.ShowMessageDialogAsync(AppResources.CannotEnterEmptyContent);
            return;
        }

        await SseSendAsync(InputTextContent);
    }

    private async Task SseSendAsync(string arg)
    {
        try
        {
            IsBusy = true;
            cancelTokenSource = new();
            if (aiApiModelBase == null)
            {
                await dialogServer.ShowMessageDialogAsync(AppResources.PleaseSelectAIModelFirst);
                return;
            }

            //IsBusy = true;
            InputTextContent = string.Empty;
            arg = arg.Trim();

            if (messageSession is null)
                throw new NullReferenceException();

            var userInputKey = await SaveChatSessionAsync(arg, Role.User);
            var newMessage = new MessagesItem()
                { Content = arg, Role = Role.User, Key = userInputKey, ShowMarkdownMode = showMarkDownState };
            messageSession.Messages.Add(newMessage);

            ChatModels.Add(new()
                { Content = arg, Role = Role.User, Key = userInputKey, ShowMarkdownMode = showMarkDownState });
            ChatItemChanged.Invoke();

            var requestService = AiRequestFactory.CreateService(messageSession, aiApiModelBase);
            var receiveMessage = new MessagesItem() { Role = Role.Assistant, StreamEnd = false };
            ChatModels.Add(receiveMessage);

            await requestService.StreamProcessResponseAsync(data =>
            {
                if (data is not UnityResponseModel result) return true;

                if (!result.Ok)
                {
                    receiveMessage.StreamEnd = true;
                    return true;
                }

                receiveMessage.Content += result.Content;
                if (result.StreamFinished)
                {
                    receiveMessage.StreamEnd = true;
                    return true;
                }

                ChatItemChanged?.Invoke();
                return false;
            }, cancelTokenSource.Token);
            if (aiApiModelBase.ContextCountLimit && messageSession.Messages.Count - 1 > aiApiModelBase.ContextCount)
                messageSession.Messages.RemoveAt(1);

            var systemOutputKey = await SaveChatSessionAsync(receiveMessage.Content ?? string.Empty, Role.Assistant);
            messageSession.Messages.Add(new()
            {
                Content = receiveMessage.Content ?? string.Empty, Role = Role.Assistant, Key = systemOutputKey,
                ShowMarkdownMode = showMarkDownState
            });
        }
        catch (Exception e)
        {
            await dialogServer.ShowMessageDialogAsync(e.Message);
        }
        finally
        {
            InputTextContent = string.Empty;
            cancelTokenSource = null;
            IsBusy = false;
        }
    }

    private async Task<Guid> SaveChatSessionAsync(string content, Role role)
    {
        var session = await chatSessionService.QueryAsync(x => x.Key == AiChatSession.Key);
        if (!session.Any())
        {
            AiChatSession.Theme = content;
            await chatSessionService.AddAsync(AiChatSession);
        }
        else
        {
            AiChatSession.Theme = content;
            await chatSessionService.UpdateAsync(AiChatSession);
        }

        // 保存对话
        var chatRecord = new AiChatMessageRecordEntity()
        {
            Content = content,
            Role = role,
            SessionId = AiChatSession.Key
        };
        await aiChatMessageRecordService.AddAsync(chatRecord);
        return chatRecord.Key;
    }


    [RelayCommand(CanExecute = nameof(IsBusyState))]
    private async Task NewChatSession()
    {
        AiChatSession.IsDefault = false;
        await chatSessionService.UpdateAsync(AiChatSession);

        AiChatSession = new AiChatSessionEntity()
        {
            IsDefault = true,
            Occupation = AssistantOccupation.Common,
            Theme = "新会话"
        };

        await chatSessionService.AddAsync(AiChatSession);
        // 清空对话记录
        await InitChatModelsAsync();
    }

    partial void OnIsBusyChanged(bool oldValue, bool newValue)
    {
        OnPropertyChanged(nameof(IsBusyState));
    }

    private bool IsBusyState()
    {
        return !IsBusy;
    }

    [RelayCommand]
    private Task SetFavouriteState(MessagesItem message)
    {
        return UpdateFavouriteStateAsync(message);
    }

    [RelayCommand]
    private async Task OpenFavouriteDialog()
    {
        await dialogServer.ShowDialogAsync<ChatMessageFavoritesViewModel, bool>(AppResources.Favourite, cancelButtonText: string.Empty);
        InitMessageSession();
        await InitChatModelsAsync();
        await InitChatHistoryAsync();

    }

    [RelayCommand(CanExecute = nameof(IsBusyState))]
    private async Task OpenSetting()
    {
        if (aiApiModelBase is null)
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.PleaseSelectAIModelFirst, AppResources.Warning);
            return;
        }

        var allSessions = await chatSessionService.QueryAsync();


        PanelSettingModel panelSettingModel = new() { AiApiModel = aiApiModelBase, AllSessions = allSessions };
        panelSettingResult =
            regionManager.AddToRegionForResult<ChatPanelSettingViewModel>("chatPanelSettingContainer",
                panelSettingModel);
        ShowPanel = true;
    }

    [RelayCommand]
    private async Task SettingConfirm()
    {
        var data = panelSettingResult?.Confirm();
        if (data is not Tuple<IEnumerable<AiChatSessionEntity>, AiApiModelBase> result)
            throw new ArgumentException();
        aiApiModelBase = result.Item2;
        await SaveAiModelAsync(aiApiModelBase);
        AiChatSession = result.Item1.FirstOrDefault(x => x.IsDefault);
        if (AiChatSession is null)
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.NoSessionSelected);

            AiChatSession = AiChatSessionEntity.CreateDefault();
            await chatSessionService.AddAsync(AiChatSession);

            goto Reset;
        }

        foreach (var item in result.Item1)
        {
            if (await chatSessionService.UpdateAsync(item)) continue;

            await DialogServer.ShowMessageDialogAsync(AppResources.FailedToUpdateSession);
            return;
        }

        Reset:
        InitMessageSession();
        await InitChatModelsAsync();
        await InitChatHistoryAsync();

        ShowPanel = false;
    }

    [RelayCommand]
    private Task SettingDismiss()
    {
        ShowPanel = false;
        return Task.CompletedTask;
    }
}