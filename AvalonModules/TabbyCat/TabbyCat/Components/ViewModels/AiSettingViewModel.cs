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

namespace TabbyCat.Components.ViewModels;

[Register]
public partial class AiSettingViewModel(
    IAiTemplateSettingService aiTemplateSettingService,
    ILogger<AiSettingViewModel> logger,
    IUser user,
    IStoreChatRecordService storeChatRecordService) : ViewModelBase
{
    [ObservableProperty] private AiApiModelBase? aiTemplate;

    [ObservableProperty] private string selectAiModelType=string.Empty;

    [ObservableProperty] private ObservableCollection<string> aiModelProviders = [];

    [ObservableProperty] private bool storeChatRecord;

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
        SelectAiModelType = AiModelProviders.First();
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
            Email = user.Email
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
                x.Provider == AiModelType.Custom && x.ModelName == customModelName && x.Email==user.Email);
            if (finds.Any()) await aiTemplateSettingService.DeleteRangeAsync(finds);
            saveModel.ModelName = customModelName;
            backSelectAiModelType = customModelName;
        }
        else
        {
            var finds = await aiTemplateSettingService.QueryAsync(x => x.Provider == AiTemplate.Provider&& x.Email==user.Email);
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
        {
            // ToastService.ShowSuccess("保存成功");
            await DialogServer.ShowMessageDialogAsync(AppResources.SavedSuccessfully,AppResources.Message,AppResources.Ok);
            await InitAiModelProvidersAsync();
            SelectAiModelType = backSelectAiModelType;
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
        if(user.LoginSuccess())
            result.AddRange(from object? item in Enum.GetValues(typeof(AiModelType)) select item.ToString()!);
        else
        {
            result.AddRange(from object? item in Enum.GetValues(typeof(AiModelType)) where item is not AiModelType.TabbyCatAi select item.ToString()!);
        }
        var customEntities = await aiTemplateSettingService.QueryAsync(x => x.Provider == AiModelType.Custom&& x.Email==user.Email);
        if (customEntities.Any())
            result.AddRange(customEntities.Select(x => x.ModelName));
        AiModelProviders.Reset(result);

    }


}