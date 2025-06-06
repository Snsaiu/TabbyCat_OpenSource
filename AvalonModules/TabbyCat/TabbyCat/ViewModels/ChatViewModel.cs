using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using Avalonia;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using TabbyCat.AiFunctionCalls;
using TabbyCat.Components.ViewModels;
using TabbyCat.Enums;
using TabbyCat.Extensions;
using TabbyCat.Factories;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests;
using TabbyCat.Models.AiReqRes.AiChatRequests.TabbyCatAi;
using TabbyCat.Models.AiReqRes.AiChatResponses;
using TabbyCat.Models.Appendix;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Shared;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Extensions;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels.Bases;
using TuDog.Bootstrap;
using TuDog.Extensions;
using TuDog.Interfaces;
using TuDog.Interfaces.Navigations;
using TuDog.Interfaces.RegionManagers;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public partial class ChatViewModel(
    IRegionManager regionManager,
    ILogger<ChatViewModel> logger,
    IStoreChatRecordService storeChatRecordService,
    INavigationService navigationService
)
    : AiViewModelBase, INavigationViewModel, IParameter, IMediaNavigation
{
    [ObservableProperty] private bool showPanel;

    [ObservableProperty] private bool isBusy;

    private CancellationTokenSource? cancelTokenSource;
    public Action? ChatItemChanged { get; set; }

    private IViewModelResult? panelSettingResult;

    [ObservableProperty] private bool canSend = true;

    [ObservableProperty] private string inputTextContent = string.Empty;

    [ObservableProperty] private ObservableCollection<AppendixModel> _appendixModels = [];

    private bool showMarkDownState;

    [ObservableProperty] private bool _useInternet;

    [ObservableProperty] private bool _useDeepThinking;

    [ObservableProperty] private ObservableCollection<AiChatSessionEntity> _chatList = [];
    
    protected override Task OnLoaded()
    {
        showMarkDownState = useMarkdownService.Get();

        if (OperatingSystem.IsWindows() || OperatingSystem.IsMacOS() || OperatingSystem.IsLinux())
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
    }


    [RelayCommand]
    private Task DeleteAppendix(AppendixModel? appendix)
    {
        AppendixModels.Reset();
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task? CancelRequestChat()
    {
        return cancelTokenSource?.CancelAsync();
    }


    private async Task InitChatHistoryAsync()
    {
        if (AiChatSession is not { } chatSession)
        {
            logger.LogError("初始化聊天历史记录时,{0}不能为空。", nameof(AiChatSession));
            return;
        }

        var histories =
            await aiChatMessageRecordService.QueryAsync(x =>
                x.SessionId == chatSession.Key);
        if (!histories.Any())
        {
            logger.LogInformation("SeesionID为{0}没有历史消息", chatSession.Key);
            return;
        }

        var orderbyTimes = histories.OrderBy(x => x.CreateTime);

        foreach (var history in orderbyTimes)
        {
            var chat = new MessagesItem()
            {
                Key = history.Key, Content = history.Content, Role = history.Role, IsFavourite = history.IsFavourite,
                ShowMarkdownMode = showMarkDownState
            };
            if (!string.IsNullOrEmpty(history.Appendix))
            {
                var model = JsonConvert.DeserializeObject<IEnumerable<AppendixModel>>(history.Appendix);
                chat.Appendixes = model;
            }

            ChatModels.Add(chat);
        }

        if (aiApiModelBase is null)
        {
            logger.LogError("初始化消息历史时,{0}变量不能为空，但是当前是空值", nameof(aiApiModelBase));
            return;
        }

        if (!aiApiModelBase.ContextCountLimit)
        {
            foreach (var chatmodel in ChatModels)
            {
                var message = new MessagesItem()
                {
                    Key = chatmodel.Key, Content = chatmodel.Content, Role = chatmodel.Role,
                    IsFavourite = chatmodel.IsFavourite, ShowMarkdownMode = showMarkDownState,
                    Appendixes = chatmodel.Appendixes
                };
                messageSession?.Messages.Add(message);
            }
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


    /// <summary>
    /// 
    /// </summary>
    /// <param name="useCommon">是否使用通用角色为默认会话</param>
    private async Task InitAiChatSessionAsync(bool useCommon = false)
    {
        var finds =
            (await chatSessionService.QueryAsync()).OrderByDescending(
                x => x.UpdateTime).ThenByDescending(x => x.IsDefault);

        if (finds.Any())
        {
            if (useCommon)
            {
               var common = finds.FirstOrDefault(x => x.Occupation == AssistantOccupation.Common );
               if (common is not null)
               {
                   var defaultItem = finds.FirstOrDefault(x => x.IsDefault );
                   if (defaultItem is not null)
                   {
                       defaultItem.IsDefault = false;
                       await chatSessionService.UpdateAsync(defaultItem);
                   }

                   common.IsDefault = true;
                   await chatSessionService.UpdateAsync(common);
                   ChatList.Reset(finds);
                   AiChatSession = common;
               }
               else
               {
                   await CreateDefaultChatSessionAsync();
               }

            }
            else
            {
                ChatList.Reset(finds);
                AiChatSession = finds.FirstOrDefault(x => x.IsDefault);
                if (AiChatSession is null)
                {
                    var f = finds.First();
                    f.IsDefault = true;
                    await chatSessionService.UpdateAsync(f);
                    AiChatSession = f;
                }
            }

            logger.LogInformation("获得默认的聊天，会话名称为:{0}。", AiChatSession.Header);
        }
        else
        {
            await CreateDefaultChatSessionAsync();
        }

        async Task CreateDefaultChatSessionAsync()
        {
            AiChatSession = AiChatSessionEntity.CreateDefault();

            ChatList.Reset([AiChatSession]);

            logger.LogInformation("没有默认的聊天会话，创建默认的会话");
            if (await chatSessionService.AddAsync(AiChatSession))
            {
                logger.LogInformation("保存默认会话成功。");
            }
            else
            {
                logger.LogError("保存默认会话失败。");
                await DialogServer.ShowMessageDialogAsync(AppResources.SaveDefaultSessionError, AppResources.Warning,
                    AppResources.Ok);
            }
        }
    }

    [RelayCommand]
    private async Task Send()
    {
        if (string.IsNullOrEmpty(InputTextContent))
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.CannotEnterEmptyContent, AppResources.Warning,
                AppResources.Ok);
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
                await DialogServer.ShowMessageDialogAsync(AppResources.PleaseSelectAIModelFirst, AppResources.Warning,
                    AppResources.Ok);
                return;
            }

            //IsBusy = true;
            InputTextContent = string.Empty;
            arg = arg.Trim();

            if (messageSession is null)
            {
                logger.LogError("发送文本内容时，{0}变量不能为空，但是此时为空", nameof(messageSession));
                await DialogServer.ShowMessageDialogAsync(AppResources.UnknownErrorSendErrorTryAgain,
                    AppResources.Warning, AppResources.Ok);
                return;
            }


            MessagesItem assistantMessage;

            if (addCurrentUserContentToMessage)
            {
                var CurrentUserInputKey = await SaveChatSessionAsync(arg, Role.User, AppendixModels.ToArray());

                if (CurrentUserInputKey is not { } key)
                {
                    await DialogServer.ShowMessageDialogAsync(AppResources.AnErrorOccurred, AppResources.Warning,
                        AppResources.Ok);
                    return;
                }

                var newMessage = MessagesItem.Create(arg, Role.User, key, useMarkdownService.Get(),
                    appendixes: AppendixModels.ToArray());
                messageSession.Messages.Add(newMessage);

                ChatModels.Add(new()
                {
                    Content = arg, Role = Role.User, Key = key, ShowMarkdownMode = useMarkdownService.Get(),
                    Appendixes = AppendixModels.ToArray()
                });
                ChatItemChanged?.Invoke();

                assistantMessage = new()
                    { Role = Role.Assistant, StreamEnd = false, ShowMarkdownMode = useMarkdownService.Get() };
                ChatModels.Add(assistantMessage);
            }
            else
            {
                var newMessage = MessagesItem.Create(arg, Role.User, Guid.Empty, useMarkdownService.Get(),
                    appendixes: AppendixModels);
                messageSession.Messages.Add(newMessage);
                assistantMessage = ChatModels.Last();
            }

            messageSession.Occupation = AiChatSession.Occupation;
            var requestService = AiRequestFactory.CreateService(messageSession, aiApiModelBase);


            AppendixModels.Reset();

            await requestService.StreamProcessResponseAsync(async data =>
            {
                if (data is not UnityResponseModel result) return true;

                if (!result.Ok)
                {
                    await DialogServer.ShowMessageDialogAsync(result.ErrorMessage, AppResources.Warning,
                        AppResources.Ok);
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
                    await SaveChatSessionAsync(assistantMessage.Content, Role.Assistant, AppendixModels);
                if (systemOutputKey is not { } key)
                {
                    await DialogServer.ShowMessageDialogAsync(AppResources.SaveChatSessionError, AppResources.Warning,
                        AppResources.Ok);
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
            logger.LogError(e, "聊天数据传输错误。");
            await DialogServer.ShowMessageDialogAsync(e.Message, AppResources.Warning, AppResources.Ok);
        }
        finally
        {
            InputTextContent = string.Empty;
            cancelTokenSource = null;
            IsBusy = false;
        }
    }

    private async Task<Guid?> SaveChatSessionAsync(string content, Role role,
        IEnumerable<AppendixModel>? appendixModels)
    {
        if (AiChatSession is not { } chatSession)
        {
            logger.LogError("保存会话时，{0}不能为空。", nameof(AiChatSession));
            return null;
        }

        var session =
            await chatSessionService.QueryAsync(x => x.Key == chatSession.Key);
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
        };

        if (appendixModels is not null && appendixModels.Any())
            chatRecord.Appendix = JsonConvert.SerializeObject(appendixModels);

        await aiChatMessageRecordService.AddAsync(chatRecord);
        return chatRecord.Key;
    }


    [RelayCommand(CanExecute = nameof(IsBusyState))]
    private async Task NewChatSession()
    {
        if (AiChatSession is null)
        {
            logger.LogError("创建新的会话时,{0}不能为空。", nameof(AiChatSession));
            return;
        }

        AiChatSession.IsDefault = false;
        await chatSessionService.UpdateAsync(AiChatSession);

        var newSession = new AiChatSessionEntity()
        {
            IsDefault = true,
            Occupation = AiChatSession.Occupation,
            Theme = "新会话",
        };

        await chatSessionService.AddAsync(newSession);
        // 清空对话记录
        await InitChatModelsAsync();
        ChatList.Insert(0, newSession);
        AiChatSession = newSession;
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
        await DialogServer.ShowDialogAsync<ChatMessageFavoritesViewModel, bool, bool>(AppResources.Favourite,
            cancelButtonText: string.Empty);
        InitMessageSession();
        await InitChatModelsAsync();
        await InitChatHistoryAsync();
    }

    [RelayCommand]
    private async Task OpenRenameSessionDialog()
    {
        if (AiChatSession is null) return;

        var name = string.IsNullOrEmpty(AiChatSession.CustomTheme)
            ? AiChatSession.Theme.Replace("\n", "").Replace("\r", "").Truncate(20)
            : AiChatSession.CustomTheme;

        var result = await DialogServer.ShowInputDialogAsync(name);
        if (result.Ok)
        {
            if (string.IsNullOrEmpty(result.Data))
            {
                await DialogServer.ShowMessageDialogAsync(AppResources.CannotEnterEmptyContent, AppResources.Warning,
                    AppResources.Ok);
                return;
            }

            AiChatSession.CustomTheme = result.Data;
            if (await chatSessionService.UpdateAsync(AiChatSession))
            {
                await DialogServer.ShowMessageDialogAsync(AppResources.RenamedSuccessfully, AppResources.Message,
                    AppResources.Ok);
            }
            else
            {
                await DialogServer.ShowMessageDialogAsync(AppResources.RenameFailed, AppResources.Warning,
                    AppResources.Ok);
                AiChatSession.CustomTheme = name;
            }
        }
        else
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.RenameFailed, AppResources.Warning, AppResources.Ok);
            AiChatSession.CustomTheme = name;
        }
    }

    [RelayCommand]
    private async Task DeleteConversation()
    {
        if (this.AiChatSession is null)
            return;

        var confirmDelete = await DialogServer.ShowConfirmDialogAsync(AppResources.ConfirmDeleteSelectedSession,
            AppResources.Message, AppResources.Ok, AppResources.Cancel);
        if (confirmDelete == false)
            return;

        if (await chatSessionService.DeleteAsync(x => x.Key == AiChatSession.Key) is not null)
        {
            if (!storeChatRecordService.Get())
                await aiChatMessageRecordService.DeleteRangeAsync(x => x.SessionId == AiChatSession.Key);

            ChatList.Remove(AiChatSession);

            if (ChatList.Any())
            {
                var firstSession = ChatList.First();
                firstSession.IsDefault = true;
                AiChatSession = firstSession;
                await chatSessionService.UpdateAsync(firstSession);
            }

            this.MessageBarService.ShowSuccess(AppResources.DeleteSuccess, AppResources.Message, true);
        }
        else
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.DeleteFailed, AppResources.Warning, AppResources.Ok);
        }
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

    public object? Parameter { get; set; }

    protected override async void SelectSessionChanged(AiChatSessionEntity? value)
    {
        InitMessageSession();
        await InitChatModelsAsync();
        await InitChatHistoryAsync();
    }

    public async Task NavigationAsync(object? parameter)
    {
        if (parameter is not QuickMenuItemModel menuItem)
        {
            return;
        }

        await GetDefaultAiTemplateModelAsync();
        await InitAiChatSessionAsync(true);
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (menuItem.Type == QuickMenuItem.Ask)
            {
                var askContent = string.Format(LocalizationResourceManager.Instance["AskTemplate"], menuItem.Content);
                await this.SseSendAsync(askContent);
            }
            else if (menuItem.Type == QuickMenuItem.Summarize)
            {
                var summaryContent = string.Format(LocalizationResourceManager.Instance["SummaryTemplate"],
                    menuItem.Content);
                await this.SseSendAsync(summaryContent);
            }
        });
    }
}