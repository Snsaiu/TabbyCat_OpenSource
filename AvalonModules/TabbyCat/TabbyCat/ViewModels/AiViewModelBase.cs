using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;
using TabbyCat.Shared.Languages;
using TabbyCat.SqliteService.AiServices;
using TuDog.Extensions;

namespace TabbyCat.ViewModels;

public abstract partial class AiViewModelBase(
    IAiTemplateSettingService aiTemplateSettingService,
    IAiChatMessageRecordService aiChatMessageRecordService) : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<string> aiModelProviders = [];

    private async Task InitAiModelProvidersAsync()
    {
        var result = new List<string>();
        foreach (var item in Enum.GetValues(typeof(AiModelType))) result.Add(item.ToString()!);
        var customEntities = await aiTemplateSettingService.QueryAsync(x => x.Provider == AiModelType.Custom);
        if (customEntities.Any())
            result.AddRange(customEntities.Select(x => x.ModelName));
        AiModelProviders.Reset(result);
    }

    protected async Task UpdateFavouriteStateAsync(MessagesItem item)
    {
        var finds = await aiChatMessageRecordService.QueryAsync(x => x.Key == item.Key);
        if (!finds.Any())
            return;
        var first = finds.First();
        first.IsFavourite = item.IsFavourite;
        first.UpdateTime = DateTime.Now;
        await aiChatMessageRecordService.UpdateAsync(first);
    }

    protected async Task SaveAiModelAsync(AiApiModelBase model)
    {
        if (model is null)
            throw new NullReferenceException();

        var json = JsonConvert.SerializeObject(model);
        var saveModel = new AiTemplateSettingEntity
        {
            Provider = model.Provider,
            IsDefault = model.IsDefault,
            Template = json
        };
        if (model.Provider == AiModelType.Custom)
        {
            var customModelName = ((IAlias)model).Alias;

            if (string.IsNullOrEmpty(customModelName))
            {
                await DialogServer.ShowMessageDialogAsync(AppResources.CustomModelMustHaveName);
                return;
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
}