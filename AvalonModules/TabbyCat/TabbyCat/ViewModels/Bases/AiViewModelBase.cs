using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TabbyCat.AiFunctionCalls;
using TabbyCat.Enums;
using TabbyCat.Factories;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;
using TabbyCat.Shared;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;
using TabbyCat.Shared.Languages;
using TuDog.Bootstrap;
using TuDog.Extensions;

namespace TabbyCat.ViewModels.Bases;

public abstract partial class AiViewModelBase : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<string> aiModelProviders = [];

    [ObservableProperty] private ObservableCollection<AiChatSessionEntity> _chatList = [];

    protected IAiTemplateSettingService AiTemplateSettingService { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IAiTemplateSettingService>();

    protected IAiChatRecordHubSyncManager AiChatRecordHubSyncManager { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IAiChatRecordHubSyncManager>();

    protected IRemoteServerService RemoteServerService { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IRemoteServerService>();

    protected IAiChatSessionService ChatSessionService { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IAiChatSessionService>();

    protected IAiChatMessageRecordService AiChatMessageRecordService { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IAiChatMessageRecordService>();

    protected ICustomAssistantOccupationService CustomAssistantOccupationService { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<ICustomAssistantOccupationService>();

    protected IAiTemplateSettingHubSyncManager AiTemplateSettingSyncManager { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IAiTemplateSettingHubSyncManager>();

    protected IChatSessionHubSyncManager ChatSessionHubSyncManager { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IChatSessionHubSyncManager>();

    protected IUseMarkdownService UseMarkdownService =
        TuDogApplication.ServiceProvider.GetRequiredService<IUseMarkdownService>();

    [ObservableProperty] private ObservableCollection<MessagesItem> chatModels = new();

    [ObservableProperty] private AiChatSessionEntity? aiChatSession;

    private ILogger<AiViewModelBase> logger =
        TuDogApplication.ServiceProvider.GetRequiredService<ILogger<AiViewModelBase>>();

    protected AiApiModelBase? aiApiModelBase;

    protected MessageSessionBase? messageSession;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="useCommon">是否使用通用角色为默认会话</param>
    protected async Task InitAiChatSessionAsync(bool useCommon = false)
    {
        var finds =
            (await ChatSessionService.QueryAsync(x => x.Email == CurrentUser.Email && !x.IsDeleted))
            .OrderByDescending(x => x.UpdateTime).ThenByDescending(x => x.IsDefault);

        if (finds.Any())
        {
            if (useCommon)
            {
                var common = finds.FirstOrDefault(x =>
                    x.Occupation == AssistantOccupation.Common && x.Email == CurrentUser.Email);
                if (common is not null)
                {
                    var defaultItem = finds.FirstOrDefault(x => x.IsDefault && x.Email == CurrentUser.Email);
                    if (defaultItem is not null)
                    {
                        defaultItem.IsDefault = false;
                        await ChatSessionService.UpdateAsync(defaultItem);
                    }

                    common.IsDefault = true;
                    await ChatSessionService.UpdateAsync(common);
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
                AiChatSession = finds.FirstOrDefault(x => x.IsDefault && x.Email == CurrentUser.Email);
                if (AiChatSession is null)
                {
                    var f = finds.First();
                    f.IsDefault = true;
                    await ChatSessionService.UpdateAsync(f);
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
            AiChatSession.Email = CurrentUser.Email;

            ChatList.Reset([AiChatSession]);

            logger.LogInformation("没有默认的聊天会话，创建默认的会话");
            if (await ChatSessionService.AddAsync(AiChatSession))
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


    protected override Task OnLoaded()
    {
        AiTemplateSettingSyncManager.UpdatedCallBack += SyncTemplateSettingUpdateAsync;
        AiChatRecordHubSyncManager.UpdatedCallBack += SyncAiChatRecordUpdateAsync;
        ChatSessionHubSyncManager.UpdatedCallBack += SyncChatSessionUpdateAsync;
        return Task.CompletedTask;
    }

    private Task SyncTemplateSettingUpdateAsync(IEnumerable<AiTemplateSettingEntity> models)
    {
        return GetDefaultAiTemplateModelAsync();
    }

    private Task SyncChatSessionUpdateAsync(IEnumerable<AiChatSessionEntity> models)
    {
        return InitAiChatSessionAsync();
    }

    private Task SyncAiChatRecordUpdateAsync(IEnumerable<AiChatMessageRecordEntity> models)
    {
        if (AiChatSession is not (not null and var session)) return Task.CompletedTask;

        var sessionId = session.SessionID;
        var chats = models.Where(x => x.SessionId == sessionId).ToArray();
        if (!chats.Any())
            return Task.CompletedTask;
        
        ChatModels.AddRange(chats.Select(x => MessagesItem.Create(x.Content, x.Role, x.Key, UseMarkdownService.Get(),
            true,
            string.IsNullOrEmpty(x.Appendix)
                ? null
                : [new AppendixModel { AppendixType = AppendixType.Image, Content = x.Appendix }])));
        return Task.CompletedTask;
    }

    protected override Task OnUnLoaded()
    {
        AiTemplateSettingSyncManager.UpdatedCallBack -= SyncTemplateSettingUpdateAsync;
        AiChatRecordHubSyncManager.UpdatedCallBack -= SyncAiChatRecordUpdateAsync;
        ChatSessionHubSyncManager.UpdatedCallBack -= SyncChatSessionUpdateAsync;
        return Task.CompletedTask;
    }

    protected async Task InitChatModelsAsync(bool addDefaultMessage = true)
    {
        ChatModels.Clear();
        messageSession?.Messages.Clear();

        if (AiChatSession is not { } chatSession)
        {
            logger.LogError("初始化聊天模型时,{0}不能为空。", nameof(AiChatSession));
            return;
        }

        if (addDefaultMessage)
        {
            if (chatSession.Occupation == AssistantOccupation.Custom)
            {
                var occupation =
                    await CustomAssistantOccupationService.QueryAsync(x =>
                        x.Name == chatSession.CustomOccupationName && x.Email == CurrentUser.Email);
                messageSession?.Messages.Add(new MessagesItem
                {
                    Content = occupation.FirstOrDefault()?.Description ?? string.Empty, Role = Role.System,
                    ShowMarkdownMode = UseMarkdownService.Get()
                });
            }
            else if (chatSession.Occupation == AssistantOccupation.Agent)
            {
                messageSession?.Messages.Add(new MessagesItem
                    { Content = AiFunctionFactory.Description(), Role = Role.System });
            }
            else
            {
                var discritpion = LocalizationResourceManager.Instance[$"{chatSession.Occupation.ToString()}Prompt"];
                messageSession?.Messages.Add(new MessagesItem
                    { Content = discritpion, Role = Role.System, ShowMarkdownMode = UseMarkdownService.Get() });
            }
        }
    }

    protected void InitMessageSession()
    {
        if (aiApiModelBase is null)
        {
            logger.LogError("初始化{0}时，{1}不能为null，但是实际{2}为空", nameof(MessageSessionBase), nameof(AiApiModelBase),
                nameof(AiApiModelBase));
            return;
        }

        messageSession = AiRequestFactory.CreateMessageSession(aiApiModelBase);
        if (messageSession is null) logger.LogError("创建消息会话失败！");
    }


    protected async Task GetDefaultAiTemplateModelAsync()
    {
        var defaultModels = await AiTemplateSettingService.QueryAsync(x => x.IsDefault && x.Email == CurrentUser.Email);

        if (!defaultModels.Any())
        {
            logger.LogWarning("查询默认的Ai聊天模板，数量是0");
            await DialogServer.ShowMessageDialogAsync(AppResources.PleaseSelectAIModelFirst, AppResources.Warning,
                AppResources.Ok);
            return;
        }

        aiApiModelBase = defaultModels.First().Provider == AiModelType.Custom
            ? await AiTemplateFactory.GetTemplateAsync(defaultModels.First().ModelName, defaultModels)
            : await AiTemplateFactory.GetTemplateAsync(defaultModels.First().Provider, defaultModels);
        if (aiApiModelBase.Provider == AiModelType.TabbyCatAi && !CurrentUser.LoginSuccess())
        {
            logger.LogWarning("默认选择的模型是TabbyCatAi，但是用户没有登录。无法使用TabbyCatAi");
            await DialogServer.ShowMessageDialogAsync(AppResources.MustLoginToUseTabbyCatAi, AppResources.Warning,
                AppResources.Ok);
            return;
        }

        logger.LogInformation("获得默认的Ai聊天模板，提供方为:{0}", aiApiModelBase.Provider.ToString());
    }

    protected async Task UpdateFavouriteStateAsync(MessagesItem item)
    {
        var finds = await AiChatMessageRecordService.QueryAsync(x => x.Key == item.Key && x.Email == CurrentUser.Email);
        if (!finds.Any())
        {
            logger.LogError("根据{0}未发现聊天历史内容", item.Key);
            return;
        }

        var first = finds.First();
        first.IsFavourite = item.IsFavourite;
        first.UpdateTime = DateTime.Now;
        if (!await AiChatMessageRecordService.UpdateAsync(first))
        {
            logger.LogError("{0}保存Favourite状态失败。", item.Key);
            return;
        }

        logger.LogInformation("{0}保存Favourite状态成功。", item.Key);
    }

    protected async Task SaveAiModelAsync(AiApiModelBase model)
    {
        var json = JsonConvert.SerializeObject(model);
        var saveModel = new AiTemplateSettingEntity
        {
            Provider = model.Provider,
            IsDefault = model.IsDefault,
            Template = json,
            Email = CurrentUser.Email
        };
        if (model.Provider == AiModelType.Custom)
        {
            var customModelName = ((IAlias)model).Alias;

            if (string.IsNullOrEmpty(customModelName))
            {
                await DialogServer.ShowMessageDialogAsync(AppResources.CustomModelMustHaveName);
                return;
            }

            var finds = await AiTemplateSettingService.QueryAsync(x =>
                x.Provider == AiModelType.Custom && x.ModelName == customModelName && x.Email == CurrentUser.Email);
            if (finds.Any()) await AiTemplateSettingService.DeleteRangeAsync(finds);
            saveModel.ModelName = customModelName;
        }
        else
        {
            var finds = await AiTemplateSettingService.QueryAsync(x =>
                x.Provider == model.Provider && x.Email == CurrentUser.Email);
            if (finds.Any()) await AiTemplateSettingService.DeleteRangeAsync(finds);
        }

        if (!saveModel.IsDefault)
        {
            var finds = await AiTemplateSettingService.QueryAsync(x => x.IsDefault && x.Email == CurrentUser.Email);
            if (!finds.Any()) saveModel.IsDefault = true;
        }
        else
        {
            var finds = await AiTemplateSettingService.QueryAsync(x => x.IsDefault && x.Email == CurrentUser.Email);
            if (finds.Any())
                foreach (var item in finds)
                {
                    item.IsDefault = false;
                    await AiTemplateSettingService.UpdateAsync(item);
                }
        }

        if (await AiTemplateSettingService.AddAsync(saveModel))
            await DialogServer.ShowMessageDialogAsync(AppResources.UpdatedSuccessfully);
        else
            await DialogServer.ShowMessageDialogAsync(AppResources.UpdatedSuccessfully);
    }

    partial void OnAiChatSessionChanged(AiChatSessionEntity? value)
    {
        SelectSessionChanged(value);
    }

    protected virtual void SelectSessionChanged(AiChatSessionEntity? value)
    {
    }
}