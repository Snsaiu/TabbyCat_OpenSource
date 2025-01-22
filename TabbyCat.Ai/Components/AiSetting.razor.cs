using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using TabbyCat.Ai.Bases;
using TabbyCat.Ai.Factories;
using TabbyCat.Ai.Models;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;
using TabbyCat.VisualBase.Bases;
using FocusEventArgs = Microsoft.AspNetCore.Components.Web.FocusEventArgs;

namespace TabbyCat.Ai.Components;

public partial class AiSetting : VisualPageBase
{
    private readonly List<AiApiModelBase> aiModels = [];
    private AiApiModelBase? selectedAiModel;

    private IEnumerable<string> aiModelProviders = [];

    private IEnumerable<string> models = [];

    [Inject] protected IAiTemplateSettingService AiTemplateSettingService { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await InitAiModelProvidersAsync();
        await SetSelectTemplateModelAsync(AiModelType.OpenAiApi.ToString());
    }

    private async Task SetSelectTemplateModelAsync(string type)
    {
        if (type == "Custom")
        {
            selectedAiModel = AiTemplateFactory.GetTemplate(AiModelType.Custom);
            return;
        }

        var templateSettingEntities = await AiTemplateSettingService.QueryAsync();
        selectedAiModel = Enum.IsDefined(typeof(AiModelType), type) && Enum.TryParse(type, out AiModelType modelType)
            ? AiTemplateFactory.GetTemplate(modelType, templateSettingEntities)
            : AiTemplateFactory.GetTemplate(type, templateSettingEntities);

        if (selectedAiModel is IHasCustomModel customModel)
            models = await customModel.GetAllModelsAsync();
        else if (selectedAiModel is IHasModels<string> hasModels) models = await hasModels.GetModelsAsync();
    }

    private async Task InitAiModelProvidersAsync()
    {
        var result = new List<string>();
        foreach (var item in Enum.GetValues(typeof(AiModelType))) result.Add(item.ToString()!);
        var customEntities = await AiTemplateSettingService.QueryAsync(x => x.Provider == AiModelType.Custom);
        if (customEntities.Any())
            result.AddRange(customEntities.Select(x => x.ModelName));
        aiModelProviders = result;
    }

    private Task SelectedOptionChangedCommand(string type)
    {
        return SetSelectTemplateModelAsync(type);
    }

    private async Task SaveCommand()
    {
        if (selectedAiModel is null)
            throw new NullReferenceException();

        var json = JsonConvert.SerializeObject(selectedAiModel);
        var saveModel = new AiTemplateSettingEntity
        {
            Provider = selectedAiModel.Provider,
            IsDefault = selectedAiModel.IsDefault,
            Template = json
        };
        if (selectedAiModel.Provider == AiModelType.Custom)
        {
            var customModelName = ((IAlias)selectedAiModel).Alias;

            if (string.IsNullOrEmpty(customModelName))
            {
                ToastService.ShowWarning("自定义模型必须要有名称");
                return;
            }

            var finds = await AiTemplateSettingService.QueryAsync(x =>
                x.Provider == AiModelType.Custom && x.ModelName == customModelName);
            if (finds.Any()) await AiTemplateSettingService.DeleteRangeAsync(finds);
            saveModel.ModelName = customModelName;
        }
        else
        {
            var finds = await AiTemplateSettingService.QueryAsync(x => x.Provider == selectedAiModel.Provider);
            if (finds.Any()) await AiTemplateSettingService.DeleteRangeAsync(finds);
        }

        if (!saveModel.IsDefault)
        {
            var finds = await AiTemplateSettingService.QueryAsync(x => x.IsDefault);
            if (!finds.Any()) saveModel.IsDefault = true;
        }
        else
        {
            var finds = await AiTemplateSettingService.QueryAsync(x => x.IsDefault);
            if (finds.Any())
                foreach (var item in finds)
                {
                    item.IsDefault = false;
                    await AiTemplateSettingService.UpdateAsync(item);
                }
        }

        if (await AiTemplateSettingService.AddAsync(saveModel))
        {
            ToastService.ShowSuccess("保存成功");
            await InitAiModelProvidersAsync();
        }
        else
        {
            ToastService.ShowError("保存失败");
        }


        //   AiService.Save(selectedAiModel as OpenAiApiModel);
    }

    private async Task DeleteCustomAiModelCommand()
    {
        if (selectedAiModel is null)
            return;

        if (selectedAiModel.IsDefault)
        {
            ToastService.ShowWarning("默认模型不能删除");
            return;
        }

        if (selectedAiModel is IAlias alias)
        {
            var finds = await AiTemplateSettingService.QueryAsync(x =>
                x.Provider == AiModelType.Custom && x.ModelName == alias.Alias);
            if (finds.Any())
            {
                await AiTemplateSettingService.DeleteRangeAsync(finds);
                await InitAiModelProvidersAsync();
                await SetSelectTemplateModelAsync(AiModelType.OpenAiApi.ToString());
            }
        }
    }

    private async Task ApiKeyChangedCommand(string arg)
    {
        if (selectedAiModel is IApiKey apikey) apikey.ApiKey = arg;

        if (selectedAiModel is IHasCustomModel customModel)
            models = await customModel.GetAllModelsAsync();
        else if (selectedAiModel is IHasModels<string> hasModels) models = await hasModels.GetModelsAsync();
    }
}