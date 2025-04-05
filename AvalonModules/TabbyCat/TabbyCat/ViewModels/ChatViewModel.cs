using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using TabbyCat.AiFunctionCalls;
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
    ILogger<ChatViewModel> logger,
    IAiChatMessageRecordService aiChatMessageRecordService)
    : AiViewModelBase(aiTemplateSettingService, aiChatMessageRecordService)
{
    [ObservableProperty] private bool showPanel;

    private AiApiModelBase? aiApiModelBase;

    [ObservableProperty] private AiChatSessionEntity? aiChatSession;

    private MessageSessionBase? messageSession;

    [ObservableProperty] private bool isBusy;

    private CancellationTokenSource? cancelTokenSource;
    public Action? ChatItemChanged { get; set; }


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
        var defaultModels = await aiTemplateSettingService.QueryAsync(x => x.IsDefault&& x.Email==user.Email);
        
        if (!defaultModels.Any())
        {
            logger.LogWarning("查询默认的Ai聊天模板，数量是0");
            await this.DialogServer.ShowMessageDialogAsync(AppResources.PleaseSelectAIModelFirst, AppResources.Warning,AppResources.Ok);
            return;
        }

        aiApiModelBase = defaultModels.First().Provider == AiModelType.Custom
            ? await AiTemplateFactory.GetTemplateAsync(defaultModels.First().ModelName, defaultModels)
            : await AiTemplateFactory.GetTemplateAsync(defaultModels.First().Provider, defaultModels);
        if (aiApiModelBase.Provider == AiModelType.TabbyCatAi && !user.LoginSuccess())
        {
            logger.LogWarning("默认选择的模型是TabbyCatAi，但是用户没有登录。无法使用TabbyCatAi");
            await dialogServer.ShowMessageDialogAsync(AppResources.MustLoginToUseTabbyCatAi,AppResources.Warning,AppResources.Ok);
            return;
        }
        logger.LogInformation("获得默认的Ai聊天模板，提供方为:{0}",aiApiModelBase.Provider.ToString());

        await InitAiChatSessionAsync();

        InitMessageSession();

        await InitChatModelsAsync();

        await InitChatHistoryAsync();
    }


    private void InitMessageSession()
    {
        if (aiApiModelBase is null)
        {
            logger.LogError("初始化{0}时，{1}不能为null，但是实际{2}为空",nameof(MessageSessionBase),nameof(AiApiModelBase),nameof(AiApiModelBase));
            return;
        }
        messageSession = AiRequestFactory.CreateMessageSession(aiApiModelBase);
        if (messageSession is null)
        {
            logger.LogError("创建消息会话失败！");
        }
        
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
        
        var histories = await aiChatMessageRecordService.QueryAsync(x => x.SessionId == chatSession.Key&& x.Email==user.Email);
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

    private async Task InitChatModelsAsync(bool addDefaultMessage = true)
    {
        ChatModels.Clear();
        messageSession?.Messages.Clear();

        if (AiChatSession is not { } chatSession)
        {
            logger.LogError("初始化聊天模型时,{0}不能为空。",nameof(AiChatSession));
            return;
        }
        
        if (addDefaultMessage)
        {
            if (chatSession.Occupation == AssistantOccupation.Custom)
            {
                var occupation =
                    await customAssistantOccupationService.QueryAsync(x =>
                        x.Name == chatSession.CustomOccupationName&& x.Email==user.Email);
                messageSession?.Messages.Add(new MessagesItem
                    { Content = occupation.FirstOrDefault()?.Description ?? string.Empty, Role = Role.System });
            }
            else if (chatSession.Occupation == AssistantOccupation.Agent)
            {
                messageSession?.Messages.Add(new() { Content = AiFunctionFactory.Description(), Role = Role.System });
            }
            else
            {
                var discritpion = LocalizationResourceManager.Instance[$"{chatSession.Occupation.ToString()}Prompt"];
                messageSession?.Messages.Add(new MessagesItem { Content = discritpion, Role = Role.System });
            }
        }
    }

    private async Task InitAiChatSessionAsync()
    {
        var finds = await chatSessionService.QueryAsync(x => x.IsDefault && x.Email==user.Email);
        if (finds.Any())
        {
            AiChatSession = finds.First();
            logger.LogInformation("获得默认的聊天，会话名称为:{0}。",AiChatSession.Header);
        }
        else
        {
            AiChatSession = AiChatSessionEntity.CreateDefault();
            AiChatSession.Email = user.Email;
            
            logger.LogInformation("没有默认的聊天会话，创建默认的会话");
            if (await chatSessionService.AddAsync(AiChatSession))
            {
                logger.LogInformation("保存默认会话成功。");
            }
            else
            {
                logger.LogError("保存默认会话失败。");
                await dialogServer.ShowMessageDialogAsync(AppResources.SaveDefaultSessionError, AppResources.Warning, AppResources.Ok);
            }
        }
    }

    [RelayCommand]
    private async Task Send()
    {
        if (string.IsNullOrEmpty(InputTextContent))
        {
            await dialogServer.ShowMessageDialogAsync(AppResources.CannotEnterEmptyContent,AppResources.Warning,AppResources.Ok);
            return;
        }

        await SseSendAsync(InputTextContent);
    }

    private string agentCommandParameter = string.Empty;

    private async Task SseSendAsync(string arg, bool addUserContentToMessage = true)
    {
        try
        {
            agentCommandParameter = string.Empty;

            IsBusy = true;
            cancelTokenSource = new();
            if (aiApiModelBase == null)
            {
                await dialogServer.ShowMessageDialogAsync(AppResources.PleaseSelectAIModelFirst,AppResources.Warning,AppResources.Ok);
                return;
            }

            //IsBusy = true;
            InputTextContent = string.Empty;
            arg = arg.Trim();

            if (messageSession is null)
            {
                logger.LogError("发送文本内容时，{0}变量不能为空，但是此时为空",nameof(messageSession));
                await dialogServer.ShowMessageDialogAsync(AppResources.UnknownErrorSendErrorTryAgain, AppResources.Warning, AppResources.Ok);
                return;
            }
               

            MessagesItem assistantMessage;

            if (addUserContentToMessage)
            {
                var userInputKey = await SaveChatSessionAsync(arg, Role.User);

                if (userInputKey is not { } key)
                {
                    await  dialogServer.ShowMessageDialogAsync(AppResources.AnErrorOccurred, AppResources.Warning, AppResources.Ok);
                    return;
                }
                
                var newMessage = new MessagesItem()
                    { Content = arg, Role = Role.User, Key = key, ShowMarkdownMode = showMarkDownState };
                messageSession.Messages.Add(newMessage);

                ChatModels.Add(new()
                    { Content = arg, Role = Role.User, Key = key, ShowMarkdownMode = showMarkDownState });
                ChatItemChanged?.Invoke();

                assistantMessage = new() { Role = Role.Assistant, StreamEnd = false };
                ChatModels.Add(assistantMessage);
            }
            else
            {
                var newMessage = new MessagesItem()
                    { Content = arg, Role = Role.User, Key = Guid.Empty, ShowMarkdownMode = showMarkDownState };
                messageSession.Messages.Add(newMessage);
                assistantMessage = ChatModels.Last();
            }


            var requestService = AiRequestFactory.CreateService(messageSession, aiApiModelBase);


            await requestService.StreamProcessResponseAsync(async data =>
            {
                if (data is not UnityResponseModel result) return true;

                if (!result.Ok)
                {
                    await dialogServer.ShowMessageDialogAsync(result.ErrorMessage, AppResources.Warning, AppResources.Ok);
                    assistantMessage.StreamEnd = true;
                    return true;
                }

                Debug.WriteLine(result.Content);
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
                   await dialogServer.ShowMessageDialogAsync(AppResources.SaveChatSessionError, AppResources.Warning, AppResources.Ok);
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
            await dialogServer.ShowMessageDialogAsync(e.Message,AppResources.Warning, AppResources.Ok);
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
        
        var session = await chatSessionService.QueryAsync(x => x.Key == chatSession.Key &&x.Email==user.Email);
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
            Email = user.Email
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
            Email = user.Email
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
            await DialogServer.ShowMessageDialogAsync(AppResources.PleaseSelectAIModelFirst, AppResources.Warning, AppResources.Ok);
            return;
        }

        var allSessions = await chatSessionService.QueryAsync(x=> x.Email==user.Email);


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
            AiChatSession.Email = user.Email;
            
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
}