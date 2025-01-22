using Microsoft.AspNetCore.Components;

using TabbyCat.Ai.Factories;
using TabbyCat.Ai.Models;
using TabbyCat.Ai.Models.AiReqRes.AiChatRequests;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;
using TabbyCat.Shared;
using TabbyCat.Shared.Enums;

namespace TabbyCat.Ai.Bases;

public abstract class AiPageComponentBase : AiComponentBase
{
    [Inject] protected IAiTemplateSettingService AiTemplateSettingService { get; set; } = null!;


    [Inject] protected IAiChatSessionService AiChatSessionService { get; set; } = null!;

    [Inject] protected IAiChatMessageRecordService AiChatMessageRecordService { get; set; } = null!;

    protected AiChatSessionEntity AiChatSession = AiChatSessionEntity.CreateDefault();


    protected AiApiModelBase? AiModel;

    protected List<MessagesItem> ChatModels = new();

    protected MessageSessionBase? MessageSession;

    private async Task InitAiChatSessionAsync()
    {
        var finds = await AiChatSessionService.QueryAsync(x => x.IsDefault);
        if (finds.Any()) AiChatSession = finds.First();
    }

    protected void InitMessageSession()
    {
        if (AiModel is null)
            return;
        MessageSession = AiRequestFactory.CreateMessageSession(AiModel);
    }

    protected async Task InitChatHistoryAsync()
    {
        var histories = await AiChatMessageRecordService.QueryAsync(x => x.SessionId == AiChatSession.Key);
        if (!histories.Any()) return;

        var orderbyTimes = histories.OrderBy(x => x.CreateTime);
        foreach (var history in orderbyTimes)
            ChatModels.Add(new MessagesItem { Content = history.Content, Role = history.Role });

        if (AiModel is null)
            return;
        if (!AiModel.ContextCountLimit)
        {
            foreach (var chatmodel in ChatModels)
                MessageSession?.Messages.Add(new MessagesItem { Content = chatmodel.Content, Role = chatmodel.Role });
        }
        else
        {
            var tasks = ChatModels.TakeLast(AiModel.ContextCount);
            foreach (var chatmodel in tasks)
                MessageSession?.Messages.Add(new MessagesItem { Content = chatmodel.Content, Role = chatmodel.Role });
        }
    }


    protected override async Task OnPageInitializedAsync(string? url, Dictionary<string, object>? data)
    {
        var defaultModels = await AiTemplateSettingService.QueryAsync(x => x.IsDefault);
        if (!defaultModels.Any())
        {
            ToastService.ShowWarning("请先配置AI模型");
            return;
        }

        AiModel = defaultModels.First().Provider == AiModelType.Custom
            ? AiTemplateFactory.GetTemplate(defaultModels.First().ModelName, defaultModels)
            : AiTemplateFactory.GetTemplate(defaultModels.First().Provider, defaultModels);

        await InitAiChatSessionAsync();

        InitMessageSession();

        await InitChatModelsAsync();

        await InitChatHistoryAsync();
    }

    protected async Task SendCommand(string arg)
    {
        await SendAsync(arg);
    }

    protected Task AfterSendAsync()
    {
        return Task.CompletedTask;
    }


    protected virtual async Task SendAsync(string arg)
    {
        try
        {
            if (AiModel == null)
            {
                ToastService.ShowWarning("请先配置AI模型");
                return;
            }

            IsBusy = true;

            arg = arg.Trim();

            if (MessageSession is null)
                throw new NullReferenceException();

            var newMessage = new MessagesItem() { Content = arg, Role = Role.User };
            MessageSession.Messages.Add(newMessage);
            ChatModels.Add(new MessagesItem { Content = arg, Role = Role.User });

            await SaveChatSessionAsync(arg, Role.User);

            var requestService = AiRequestFactory.CreateService(MessageSession, AiModel);
            var responseData = await requestService.ProcessRequestAsync();
            if (responseData is null)
            {
                ToastService.ShowWarning("发生错误，请稍后再试");
                return;
            }
            if (responseData.Ok == false)
            {
                ToastService.ShowWarning(responseData.ErrorMessage);
                return;
            }

            ChatModels.Add(new MessagesItem { Content = responseData.Content ?? string.Empty, Role = Role.Assistant });
            if (AiModel.ContextCountLimit && MessageSession.Messages.Count - 1 > AiModel.ContextCount)
                MessageSession.Messages.RemoveAt(1);
            MessageSession.Messages.Add(new MessagesItem
            { Content = responseData.Content ?? string.Empty, Role = Role.Assistant });
            await SaveChatSessionAsync(responseData.Content ?? string.Empty, Role.Assistant);
        }
        catch (Exception e)
        {
            ToastService.ShowError(e.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveChatSessionAsync(string content, Role role)
    {
        var session = await AiChatSessionService.QueryAsync(x => x.Key == AiChatSession.Key);
        if (!session.Any())
        {
            AiChatSession.Theme = content;
            await AiChatSessionService.AddAsync(AiChatSession);
        }
        else
        {
            AiChatSession.Theme = content;
            await AiChatSessionService.UpdateAsync(AiChatSession);
        }

        // 保存对话
        var chatRecord = new AiChatMessageRecordEntity()
        {
            Content = content,
            Role = role,
            SessionId = AiChatSession.Key
        };
        await AiChatMessageRecordService.AddAsync(chatRecord);
    }

    protected async Task NewChatSessionCommand()
    {
        AiChatSession.IsDefault = false;
        await AiChatSessionService.UpdateAsync(AiChatSession);

        AiChatSession = new AiChatSessionEntity()
        {
            IsDefault = true,
            Occupation = AssistantOccupation.Common,
            Theme = "新会话"
        };

        await AiChatSessionService.AddAsync(AiChatSession);
        // 清空对话记录
        await InitChatModelsAsync();
    }

    protected async Task InitChatModelsAsync(bool addDefaultMessage = true)
    {
        ChatModels.Clear();
        MessageSession?.Messages.Clear();
        if (addDefaultMessage)
        {
            if (AiChatSession.Occupation == AssistantOccupation.Custom)
            {
                var occupation =
                    await CustomAssistantOccupationService.QueryAsync(x =>
                        x.Name == AiChatSession.CustomOccupationName);
                MessageSession?.Messages.Add(new MessagesItem
                { Content = occupation.FirstOrDefault()?.Description ?? string.Empty, Role = Role.System });
            }
            else
            {
                var discritpion = LocalizationResourceManager.Instance[$"{AiChatSession.Occupation.ToString()}Prompt"];
                MessageSession?.Messages.Add(new MessagesItem { Content = discritpion, Role = Role.System });
            }
        }
    }
}