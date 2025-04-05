using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;
using TabbyCat.Shared.Languages;
using TabbyCat.SqliteService.AiServices;
using TuDog.Bootstrap;

namespace TabbyCat.ViewModels;

public abstract partial class AiViewModelBase(
    IAiTemplateSettingService aiTemplateSettingService,
    IAiChatMessageRecordService aiChatMessageRecordService) : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<string> aiModelProviders = [];

    protected IUser user = TuDogApplication.ServiceProvider.GetRequiredService<IUser>();

    private ILogger<AiViewModelBase> logger = TuDogApplication.ServiceProvider.GetRequiredService<ILogger<AiViewModelBase>>();

    protected async Task UpdateFavouriteStateAsync(MessagesItem item)
    {
        var finds = await aiChatMessageRecordService.QueryAsync(x => x.Key == item.Key&& x.Email==user.Email);
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
            Email = user.Email
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
                x.Provider == AiModelType.Custom && x.ModelName == customModelName&& x.Email==user.Email);
            if (finds.Any()) await aiTemplateSettingService.DeleteRangeAsync(finds);
            saveModel.ModelName = customModelName;
        }
        else
        {
            var finds = await aiTemplateSettingService.QueryAsync(x => x.Provider == model.Provider&& x.Email==user.Email);
            if (finds.Any()) await aiTemplateSettingService.DeleteRangeAsync(finds);
        }

        if (!saveModel.IsDefault)
        {
            var finds = await aiTemplateSettingService.QueryAsync(x => x.IsDefault&& x.Email==user.Email);
            if (!finds.Any()) saveModel.IsDefault = true;
        }
        else
        {
            var finds = await aiTemplateSettingService.QueryAsync(x => x.IsDefault&& x.Email==user.Email);
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
}