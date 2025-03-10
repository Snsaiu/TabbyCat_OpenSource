using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TabbyCat.Factories;
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

namespace TabbyCat.Components.ViewModels;

[Register]
public partial class AiSettingViewModel(
    IAiTemplateSettingService aiTemplateSettingService,
    IStoreChatRecordService storeChatRecordService) : ViewModelBase
{
    [ObservableProperty] private AiApiModelBase aiTemplate;

    [ObservableProperty] private string selectAiModelType;

    [ObservableProperty] private ObservableCollection<string> aiModelProviders = [];

    [ObservableProperty] private bool storeChatRecord;

    [RelayCommand]
    private async Task RefreshModel()
    {
        if (AiTemplate is IHasModels<string> hasModels)
        {
            var models = await hasModels.GetModelsAsync();
            hasModels.Models.Reset(models);

            hasModels.SelectedModel = models is null ? string.Empty : models.FirstOrDefault();
        }
    }

    protected override async Task OnLoaded()
    {
        StoreChatRecord = storeChatRecordService.Get();

        await InitAiModelProvidersAsync();
        SelectAiModelType = AiModelProviders.First();
        AiTemplate = await AiTemplateFactory.GetTemplateAsync(AiModelType.OpenAiApi);
    }

    partial void OnStoreChatRecordChanged(bool value)
    {
        storeChatRecordService.Set(value);
    }




    partial void OnSelectAiModelTypeChanged(string oldValue, string newValue)
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
            Template = json
        };
        if (AiTemplate.Provider == AiModelType.Custom)
        {
            var customModelName = ((IAlias)AiTemplate).Alias;

            if (string.IsNullOrEmpty(customModelName))
            {
                // ToastService.ShowWarning("自定义模型必须要有名称");
                await DialogServer.ShowMessageDialogAsync(AppResources.CustomModelMustHaveName);
                return;
            }

            var finds = await aiTemplateSettingService.QueryAsync(x =>
                x.Provider == AiModelType.Custom && x.ModelName == customModelName);
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
            await DialogServer.ShowMessageDialogAsync(AppResources.SavedSuccessfully);
            await InitAiModelProvidersAsync();
            SelectAiModelType = backSelectAiModelType;
        }
        else
        {
            await DialogServer.ShowMessageDialogAsync(AppResources.SaveFailed);
            // ToastService.ShowError("保存失败");
        }

    }

    private async Task InitAiModelProvidersAsync()
    {
        var result = new List<string>();
        foreach (var item in Enum.GetValues(typeof(AiModelType))) result.Add(item.ToString()!);
        var customEntities = await aiTemplateSettingService.QueryAsync(x => x.Provider == AiModelType.Custom);
        if (customEntities.Any())
            result.AddRange(customEntities.Select(x => x.ModelName));
        AiModelProviders.Reset(result);

    }


}