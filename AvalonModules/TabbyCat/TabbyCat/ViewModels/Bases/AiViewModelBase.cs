using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TabbyCat.AiFunctionCalls;
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

namespace TabbyCat.ViewModels.Bases;

public abstract partial class AiViewModelBase : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<string> aiModelProviders = [];

    protected IAiTemplateSettingService aiTemplateSettingService =
        TuDogApplication.ServiceProvider.GetRequiredService<IAiTemplateSettingService>();

    protected IRemoteServerService RemoteServerService { get; } =
        TuDogApplication.ServiceProvider.GetRequiredService<IRemoteServerService>();

    protected IAiChatSessionService chatSessionService =
        TuDogApplication.ServiceProvider.GetRequiredService<IAiChatSessionService>();

    protected IAiChatMessageRecordService aiChatMessageRecordService=TuDogApplication.ServiceProvider.GetRequiredService<IAiChatMessageRecordService>();

    protected ICustomAssistantOccupationService customAssistantOccupationService=TuDogApplication.ServiceProvider.GetRequiredService<ICustomAssistantOccupationService>();


    protected IUseMarkdownService useMarkdownService=TuDogApplication.ServiceProvider.GetRequiredService<IUseMarkdownService>();

    [ObservableProperty]
    private ObservableCollection<MessagesItem> chatModels = new();

    [ObservableProperty] private AiChatSessionEntity? aiChatSession;



    private ILogger<AiViewModelBase> logger = TuDogApplication.ServiceProvider.GetRequiredService<ILogger<AiViewModelBase>>();


    protected AiApiModelBase? aiApiModelBase;

    protected MessageSessionBase? messageSession;


    protected async Task InitChatModelsAsync(bool addDefaultMessage = true)
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
                        x.Name == chatSession.CustomOccupationName);
                messageSession?.Messages.Add(new MessagesItem
                {
                    Content = occupation.FirstOrDefault()?.Description ?? string.Empty, Role = Role.System,
                    ShowMarkdownMode = useMarkdownService.Get()
                });
            }
            else if (chatSession.Occupation == AssistantOccupation.Agent)
            {
                messageSession?.Messages.Add(new() { Content = AiFunctionFactory.Description(), Role = Role.System });
            }
            else
            {
                var discritpion = LocalizationResourceManager.Instance[$"{chatSession.Occupation.ToString()}Prompt"];
                messageSession?.Messages.Add(new()
                    { Content = discritpion, Role = Role.System, ShowMarkdownMode = useMarkdownService.Get() });
            }
        }
    }

    protected void InitMessageSession()
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


    protected async Task GetDefaultAiTemplateModelAsync()
    {
        var defaultModels = await aiTemplateSettingService.QueryAsync(x => x.IsDefault);

        if (!defaultModels.Any())
        {
            logger.LogWarning("查询默认的Ai聊天模板，数量是0");
            await this.DialogServer.ShowMessageDialogAsync(AppResources.PleaseSelectAIModelFirst, AppResources.Warning,AppResources.Ok);
            return;
        }

        aiApiModelBase = defaultModels.First().Provider == AiModelType.Custom
            ? await AiTemplateFactory.GetTemplateAsync(defaultModels.First().ModelName, defaultModels)
            : await AiTemplateFactory.GetTemplateAsync(defaultModels.First().Provider, defaultModels);
      
        logger.LogInformation("获得默认的Ai聊天模板，提供方为:{0}",aiApiModelBase.Provider.ToString());
    }

    protected async Task UpdateFavouriteStateAsync(MessagesItem item)
    {
        var finds = await aiChatMessageRecordService.QueryAsync(x => x.Key == item.Key);
        if (!finds.Any())
        {
            logger.LogError("根据{0}未发现聊天历史内容",item.Key);
            return;
        }
        var first = finds.First();
        first.IsFavourite = item.IsFavourite;
        first.UpdateTime = DateTime.Now;
        if (!await aiChatMessageRecordService.UpdateAsync(first))
        {
            logger.LogError("{0}保存Favourite状态失败。",item.Key);
            return;
        }
        logger.LogInformation("{0}保存Favourite状态成功。",item.Key);
    }

    protected async Task SaveAiModelAsync(AiApiModelBase model)
    {
        var json = JsonConvert.SerializeObject(model);
        var saveModel = new AiTemplateSettingEntity
        {
            Provider = model.Provider,
            IsDefault = model.IsDefault,
            Template = json,
        };
        if (model.Provider == AiModelType.Custom)
        {
            var customModelName = ((IAlias)model).Alias;

            if (string.IsNullOrEmpty(customModelName))
            {
                await DialogServer.ShowMessageDialogAsync(AppResources.CustomModelMustHaveName);
                return ;
            }

            var finds = await aiTemplateSettingService.QueryAsync(x =>
                x.Provider == AiModelType.Custom && x.ModelName == customModelName);
            if (finds.Any()) await aiTemplateSettingService.DeleteRangeAsync(finds);
            saveModel.ModelName = customModelName;
        }
        else
        {
            var finds = await aiTemplateSettingService.QueryAsync(x => x.Provider == model.Provider);
            if (finds.Any()) await aiTemplateSettingService.DeleteRangeAsync(finds);
        }

        if (!saveModel.IsDefault)
        {
            var finds = await aiTemplateSettingService.QueryAsync(x => x.IsDefault);
            if (!finds.Any()) saveModel.IsDefault = true;
        }
        else
        {
            var finds = await aiTemplateSettingService.QueryAsync(x => x.IsDefault);
            if (finds.Any())
                foreach (var item in finds)
                {
                    item.IsDefault = false;
                    await aiTemplateSettingService.UpdateAsync(item);
                }
        }

        if (await aiTemplateSettingService.AddAsync(saveModel))
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