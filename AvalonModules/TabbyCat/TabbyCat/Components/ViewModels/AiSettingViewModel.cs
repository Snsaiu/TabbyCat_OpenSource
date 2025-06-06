using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using TabbyCat.Factories;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels;
using TuDog.Extensions;
using TuDog.IocAttribute;
using ViewModelBase = TabbyCat.ViewModels.Bases.ViewModelBase;

namespace TabbyCat.Components.ViewModels;

[Register]
public partial class AiSettingViewModel(
    IAiTemplateSettingService aiTemplateSettingService,
    ILogger<AiSettingViewModel> logger,
    IAiTemplateSettingSyncService aiTemplateSettingSyncService,
    IStoreChatRecordService storeChatRecordService) : ViewModelBase
{
    [ObservableProperty] private AiApiModelBase? _aiTemplate;

    [ObservableProperty] private string _selectAiModelType = string.Empty;

    [ObservableProperty] private ObservableCollection<string> _aiModelProviders = [];

    [ObservableProperty] private bool _storeChatRecord;

    [RelayCommand]
    private async Task RefreshModel()
    {
        logger.LogInformation("刷新模型。");
        if (AiTemplate is IHasModels<string> hasModels)
        {
            var models = await hasModels.GetModelsAsync();
            hasModels.Models.Reset(models);

            hasModels.SelectedModel = models.FirstOrDefault()??string.Empty;
            if (string.IsNullOrEmpty(hasModels.SelectedModel))
            {
                logger.LogWarning("没有任何模型名称被选中。");
            }

        }
    }

    protected override async Task OnLoaded()
    {
        StoreChatRecord = storeChatRecordService.Get();

        await InitAiModelProvidersAsync();
        AiTemplate = await AiTemplateFactory.GetTemplateAsync(AiModelType.OpenAiApi);
    }

    partial void OnStoreChatRecordChanged(bool value)
    {
        storeChatRecordService.Set(value);
    }

    partial void OnSelectAiModelTypeChanged(string? oldValue, string newValue)
    {
        if (newValue == "Custom")
        {
            Task.Run(async () => { AiTemplate = await AiTemplateFactory.GetTemplateAsync(AiModelType.Custom); });
            return;
        }

        Task.Run(async () =>
        {
            var templateSettingEntities = await aiTemplateSettingService.QueryAsync();
            AiTemplate = Enum.IsDefined(typeof(AiModelType), newValue) &&
                         Enum.TryParse(newValue, out AiModelType modelType)
                ? await AiTemplateFactory.GetTemplateAsync(modelType, templateSettingEntities)
                : await AiTemplateFactory.GetTemplateAsync(newValue, templateSettingEntities);
        });
    }

    [RelayCommand]
    private async Task Save()
    {
        if (AiTemplate is null)
            throw new NullReferenceException();

        var backSelectAiModelType = SelectAiModelType;
        var json = JsonConvert.SerializeObject(AiTemplate);

        var saveModel = new AiTemplateSettingEntity
        {
            Provider = AiTemplate.Provider,
            IsDefault = AiTemplate.IsDefault,
            Template = json,
        };



        if (AiTemplate.Provider == AiModelType.Custom)
        {
            var customModelName = ((IAlias)AiTemplate).Alias;

            if (string.IsNullOrEmpty(customModelName))
            {
                // ToastService.ShowWarning("自定义模型必须要有名称");
                await DialogServer.ShowMessageDialogAsync(AppResources.CustomModelMustHaveName,AppResources.Message,AppResources.Ok);
                return;
            }

            var finds = await aiTemplateSettingService.QueryAsync(x =>
                x.Provider == AiModelType.Custom && x.ModelName == customModelName );
            if (finds.Any()) await aiTemplateSettingService.DeleteRangeAsync(finds);
            saveModel.ModelName = customModelName;
            backSelectAiModelType = customModelName;
        }
        else
        {
            var finds = await aiTemplateSettingService.QueryAsync(x => x.Provider == AiTemplate.Provider);
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
        {
            // ToastService.ShowSuccess("保存成功");
            await DialogServer.ShowMessageDialogAsync(AppResources.SavedSuccessfully,AppResources.Message,AppResources.Ok);
            await InitAiModelProvidersAsync();
        }
        else
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.SaveFailed,AppResources.Warning,AppResources.Ok);
            // ToastService.ShowError("保存失败");
        }

    }
    private async Task InitAiModelProvidersAsync()
    {
        var result = new List<string>();

        result.AddRange(from object? item in Enum.GetValues(typeof(AiModelType)) select item.ToString()!);
        
        var aiTemplates = await aiTemplateSettingService.QueryAsync();
        var customEntities = aiTemplates.Where(x => x.Provider == AiModelType.Custom);
        if (customEntities.Any())
            result.AddRange(customEntities.Select(x => x.ModelName));
        AiModelProviders.Reset(result);
        if (aiTemplates.FirstOrDefault(x => x.IsDefault) is { } defaultEntity)
            SelectAiModelType = defaultEntity.Provider.ToString();
        else
        {
            SelectAiModelType = AiModelProviders.First();
        }
    }


}