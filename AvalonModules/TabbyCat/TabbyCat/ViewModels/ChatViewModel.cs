using System.Diagnostics;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using TabbyCat.AiFunctionCalls;
using TabbyCat.Components.ViewModels;
using TabbyCat.Factories;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests;
using TabbyCat.Models.AiReqRes.AiChatResponses;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Languages;
using TuDog.Interfaces;
using TuDog.Interfaces.Navigations;
using TuDog.Interfaces.RegionManagers;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public partial class ChatViewModel(
    IRegionManager regionManager,
    ILogger<ChatViewModel> logger,
    INavigationService navigationService
   )
    : AiViewModelBase,INavigationViewModel
{
    [ObservableProperty] private bool showPanel;
    
    [ObservableProperty] private bool isBusy;

    private CancellationTokenSource? cancelTokenSource;
    public Action? ChatItemChanged { get; set; }
    
    private IViewModelResult? panelSettingResult;

    [ObservableProperty]
    private string inputTextContent = string.Empty;

    private bool showMarkDownState;

    protected override Task OnLoaded()
    {
        showMarkDownState = useMarkdownService.Get();
        
        if(OperatingSystem.IsWindows()||OperatingSystem.IsMacOS()||OperatingSystem.IsLinux())
            return InitializedAsync();
        
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task ReturnPage()
    {
       return navigationService.PopAsync(null);
    }
    
    private async Task InitializedAsync()
    {
        await GetDefaultAiTemplateModelAsync();
        await InitAiChatSessionAsync();

        InitMessageSession();

        await InitChatModelsAsync();

        await InitChatHistoryAsync();
    }


   
    [RelayCommand]
    private Task? CancelRequestChat()
    {
        return cancelTokenSource?.CancelAsync();
    }

    private async Task InitChatHistoryAsync()
    {
        if (AiChatSession is not {} chatSession)
        {
            logger.LogError("初始化聊天历史记录时,{0}不能为空。",nameof(AiChatSession));
            return;
        }

        var histories = await aiChatMessageRecordService.QueryAsync(x => x.SessionId == chatSession.Key&& x.Email==CurrentUser.Email);
        if (!histories.Any())
        {
            logger.LogInformation("SeesionID为{0}没有历史消息",chatSession.Key);
            return;
        }

        var orderbyTimes = histories.OrderBy(x => x.CreateTime);
        foreach (var history in orderbyTimes)
            ChatModels.Add(new()
            {
                Key = history.Key, Content = history.Content, Role = history.Role, IsFavourite = history.IsFavourite,
                ShowMarkdownMode = showMarkDownState
            });

        if (aiApiModelBase is null)
        {
            logger.LogError("初始化消息历史时,{0}变量不能为空，但是当前是空值",nameof(aiApiModelBase));
            return;
        }

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

   

    private async Task InitAiChatSessionAsync()
    {
        var finds = await chatSessionService.QueryAsync(x => x.IsDefault && x.Email==CurrentUser.Email);
        if (finds.Any())
        {
            AiChatSession = finds.First();
            logger.LogInformation("获得默认的聊天，会话名称为:{0}。",AiChatSession.Header);
        }
        else
        {
            AiChatSession = AiChatSessionEntity.CreateDefault();
            AiChatSession.Email = CurrentUser.Email;

            logger.LogInformation("没有默认的聊天会话，创建默认的会话");
            if (await chatSessionService.AddAsync(AiChatSession))
            {
                logger.LogInformation("保存默认会话成功。");
            }
            else
            {
                logger.LogError("保存默认会话失败。");
                await DialogServer.ShowMessageDialogAsync(AppResources.SaveDefaultSessionError, AppResources.Warning, AppResources.Ok);
            }
        }
    }

    [RelayCommand]
    private async Task Send()
    {
        if (string.IsNullOrEmpty(InputTextContent))
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.CannotEnterEmptyContent,AppResources.Warning,AppResources.Ok);
            return;
        }

        await SseSendAsync(InputTextContent);
    }

    private string agentCommandParameter = string.Empty;

    private async Task SseSendAsync(string arg, bool addCurrentUserContentToMessage = true)
    {
        try
        {
            agentCommandParameter = string.Empty;

            IsBusy = true;
            cancelTokenSource = new();
            if (aiApiModelBase == null)
            {
                await DialogServer.ShowMessageDialogAsync(AppResources.PleaseSelectAIModelFirst,AppResources.Warning,AppResources.Ok);
                return;
            }

            //IsBusy = true;
            InputTextContent = string.Empty;
            arg = arg.Trim();

            if (messageSession is null)
            {
                logger.LogError("发送文本内容时，{0}变量不能为空，但是此时为空",nameof(messageSession));
                await DialogServer.ShowMessageDialogAsync(AppResources.UnknownErrorSendErrorTryAgain, AppResources.Warning, AppResources.Ok);
                return;
            }


            MessagesItem assistantMessage;

            if (addCurrentUserContentToMessage)
            {
                var CurrentUserInputKey = await SaveChatSessionAsync(arg, Role.User);

                if (CurrentUserInputKey is not { } key)
                {
                    await  DialogServer.ShowMessageDialogAsync(AppResources.AnErrorOccurred, AppResources.Warning, AppResources.Ok);
                    return;
                }

                var newMessage = new MessagesItem()
                    { Content = arg, Role = Role.User, Key = key, ShowMarkdownMode = useMarkdownService.Get() };
                messageSession.Messages.Add(newMessage);

                ChatModels.Add(new()
                    { Content = arg, Role = Role.User, Key = key, ShowMarkdownMode = useMarkdownService.Get() });
                ChatItemChanged?.Invoke();

                assistantMessage = new()
                    { Role = Role.Assistant, StreamEnd = false, ShowMarkdownMode = useMarkdownService.Get() };
                ChatModels.Add(assistantMessage);
            }
            else
            {
                var newMessage = new MessagesItem()
                    { Content = arg, Role = Role.User, Key = Guid.Empty, ShowMarkdownMode = useMarkdownService.Get() };
                messageSession.Messages.Add(newMessage);
                assistantMessage = ChatModels.Last();
            }


            var requestService = AiRequestFactory.CreateService(messageSession, aiApiModelBase);


            await requestService.StreamProcessResponseAsync(async data =>
            {
                if (data is not UnityResponseModel result) return true;

                if (!result.Ok)
                {
                    await DialogServer.ShowMessageDialogAsync(result.ErrorMessage, AppResources.Warning, AppResources.Ok);
                    assistantMessage.StreamEnd = true;
                    return true;
                }
                
                if (AiChatSession?.Occupation == AssistantOccupation.Agent)
                {
                    agentCommandParameter += result.Content;
                    if (!agentCommandParameter.StartsWith("{"))
                    {
                        assistantMessage.Content += result.Content;

                        if (result.StreamFinished)
                        {
                            assistantMessage.StreamEnd = true;
                            return true;
                        }
                    }
                    else
                    {
                        if (result.StreamFinished)
                        {
                            // 调用功能
                            var functionResult = await AiFunctionFactory.QueryAsync(agentCommandParameter);
                            if (functionResult is null)
                                throw new NullReferenceException();
                            await SseSendAsync(JsonConvert.SerializeObject(functionResult), false);
                            assistantMessage.StreamEnd = true;
                            return true;
                        }
                    }
                }
                else
                {
                    assistantMessage.Content += result.Content;
                    if (result.StreamFinished)
                    {
                        assistantMessage.StreamEnd = true;
                        return true;
                    }
                }
                ChatItemChanged?.Invoke();
                return false;
            }, cancelTokenSource.Token);

            messageSession.Messages.RemoveAll(x =>
                (x.Key == Guid.Empty && x.Role != Role.System) || string.IsNullOrEmpty(x.Content));

            if (aiApiModelBase.ContextCountLimit && messageSession.Messages.Count - 1 > aiApiModelBase.ContextCount)
                messageSession.Messages.RemoveAt(1);

            if (!string.IsNullOrEmpty(assistantMessage.Content))
            {
                var systemOutputKey =
                    await SaveChatSessionAsync(assistantMessage.Content, Role.Assistant);
                if (systemOutputKey is not {} key)
                {
                   await DialogServer.ShowMessageDialogAsync(AppResources.SaveChatSessionError, AppResources.Warning, AppResources.Ok);
                   return;
                }
                messageSession.Messages.Add(new()
                {
                    Content = assistantMessage.Content, Role = Role.Assistant, Key = key,
                    ShowMarkdownMode = showMarkDownState
                });
            }
        }
        catch (Exception e)
        {
            logger.LogError(e,"聊天数据传输错误。");
            await DialogServer.ShowMessageDialogAsync(e.Message,AppResources.Warning, AppResources.Ok);
        }
        finally
        {
            InputTextContent = string.Empty;
            cancelTokenSource = null;
            IsBusy = false;
        }
    }

    private async Task<Guid?> SaveChatSessionAsync(string content, Role role)
    {
        if (AiChatSession is not { } chatSession)
        {
            logger.LogError("保存会话时，{0}不能为空。",nameof(AiChatSession));
            return null;
        }

        var session = await chatSessionService.QueryAsync(x => x.Key == chatSession.Key &&x.Email==CurrentUser.Email);
        if (!session.Any())
        {
            chatSession.Theme = content;
            await chatSessionService.AddAsync(chatSession);
        }
        else
        {
            chatSession.Theme = content;
            await chatSessionService.UpdateAsync(chatSession);
        }

        // 保存对话
        var chatRecord = new AiChatMessageRecordEntity()
        {
            Content = content,
            Role = role,
            SessionId = chatSession.Key,
            Email = CurrentUser.Email
        };
        await aiChatMessageRecordService.AddAsync(chatRecord);
        return chatRecord.Key;
    }


    [RelayCommand(CanExecute = nameof(IsBusyState))]
    private async Task NewChatSession()
    {
        if (AiChatSession is not { } )
        {
            logger.LogError("创建新的会话时,{0}不能为空。",nameof(AiChatSession));
            return;
        }

        AiChatSession.IsDefault = false;
        await chatSessionService.UpdateAsync(AiChatSession);

        AiChatSession = new AiChatSessionEntity()
        {
            IsDefault = true,
            Occupation = AssistantOccupation.Common,
            Theme = "新会话",
            Email = CurrentUser.Email
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
        await DialogServer.ShowDialogAsync<ChatMessageFavoritesViewModel, bool>(AppResources.Favourite, cancelButtonText: string.Empty);
        InitMessageSession();
        await InitChatModelsAsync();
        await InitChatHistoryAsync();

    }

    [RelayCommand(CanExecute = nameof(IsBusyState))]
    private async Task OpenSetting()
    {
        if (aiApiModelBase is null)
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.PleaseSelectAIModelFirst, AppResources.Warning, AppResources.Ok);
            return;
        }

        var allSessions = await chatSessionService.QueryAsync(x=> x.Email==CurrentUser.Email);


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

        if (result.Item1.FirstOrDefault(x => x.IsDefault) is not { } defaultChat)
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.NoSessionSelected,AppResources.Message, AppResources.Ok);

            AiChatSession = AiChatSessionEntity.CreateDefault();
            AiChatSession.Email = CurrentUser.Email;

            await chatSessionService.AddAsync(AiChatSession);
            goto Reset;
        }

        AiChatSession = defaultChat;

        foreach (var item in result.Item1)
        {
            if (await chatSessionService.UpdateAsync(item)) continue;

            await DialogServer.ShowMessageDialogAsync(AppResources.FailedToUpdateSession,AppResources.Warning, AppResources.Ok);
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

    public async Task OnPushHereAsync(INavigationParameter? parameter)
    {
        if (parameter is null)
            return;
        if (parameter.TryAndGet("New", out bool isNew))
        {
           await GetDefaultAiTemplateModelAsync();
            if (isNew)
            {
                if (!parameter.TryAndGet("Occupation", out AssistantOccupation occupation))
                {
                    throw new NullReferenceException();
                }

                AiChatSession = AiChatSessionEntity.CreateDefault(occupation);
                AiChatSession.Email = CurrentUser.Email;

                if (await chatSessionService.AddAsync(AiChatSession))
                {
                    logger.LogInformation("保存默认会话成功。");
                }
                else
                {
                    logger.LogError("保存默认会话失败。");
                    await DialogServer.ShowMessageDialogAsync(AppResources.SaveDefaultSessionError,
                        AppResources.Warning, AppResources.Ok);
                    return;
                }
            }
            else
            {
               await InitAiChatSessionAsync();
            }
            
            InitMessageSession();
            await InitChatModelsAsync();
            await InitChatHistoryAsync();
        }
        else
        {
            throw new NullReferenceException();
        }
        
    }

    public Task OnPopHereAsync(INavigationParameter? result)
    {
        throw new NotImplementedException();
    }
}