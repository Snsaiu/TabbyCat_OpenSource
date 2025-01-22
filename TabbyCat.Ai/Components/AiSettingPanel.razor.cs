using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using TabbyCat.Ai.Bases;
using TabbyCat.Ai.Models;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.Ai.Components;

public partial class AiSettingPanel : AiComponentBase, IDialogContentComponent
{
    [Parameter] public Tuple<List<AiChatSessionEntity>, AiApiModelBase> Content { get; set; }

    [CascadingParameter] public FluentDialog Dialog { get; set; } = default!;


    private AiChatSessionEntity? selectedAiChatSessionEntity;

    private OccupationType? selectedOccupationType;

    private int selectOccupationVersion = 0;
    private int selectSessionVersion = 0;


    private IEnumerable<OccupationType> occupations = [];

    private List<string> models = [];

    private string selectedModel = string.Empty;

    #region 创建自定义角色字段

    private string newOccupationName = string.Empty;

    private string newOccupationDescription = string.Empty;

    private bool newOccupationIsDefault = false;

    #endregion


    [Inject] private IAiChatSessionService AiChatSessionService { get; set; } = null!;


    protected override async Task OnPageInitializedAsync(string? url, Dictionary<string, object>? data)
    {
        selectedAiChatSessionEntity = Content.Item1.FirstOrDefault(x => x.IsDefault);
        if (Content.Item2 is IHasModels<string> hasModelsModel)
        {
            models.AddRange(await hasModelsModel.GetModelsAsync());
            selectedModel = hasModelsModel.SelectedModel;
        }

        occupations = await GetAllOccupationsAsync();
        if (selectedAiChatSessionEntity.Occupation != AssistantOccupation.Custom)
            selectedOccupationType =
                occupations.FirstOrDefault(x => selectedAiChatSessionEntity.Occupation == x.Occupation);
        else
            selectedOccupationType = occupations.FirstOrDefault(x =>
                x.OccupationName == selectedAiChatSessionEntity.CustomOccupationName);

        if (selectedOccupationType is null)
            selectedOccupationType = occupations.FirstOrDefault();
    }

    private void SelectedSessionChangedCommand(bool selected, AiChatSessionEntity? obj)
    {
        if (obj is null)
            return;
        Content.Item1.ForEach(x => { x.IsDefault = x.Key == obj.Key; });


        if (obj.Occupation == AssistantOccupation.Custom)
            selectedOccupationType = occupations.FirstOrDefault(x =>
                x.Occupation == AssistantOccupation.Custom &&
                x.OccupationName == obj.CustomOccupationName);
        else
            selectedOccupationType = occupations.FirstOrDefault(x =>
                x.Occupation == obj.Occupation);
        selectedAiChatSessionEntity = obj;
        selectSessionVersion++;
    }

    private async Task RenameSessionCommand(AiChatSessionEntity item)
    {
        var result = await DialogService.ShowDialogAsync<RenameSession>(
            string.IsNullOrEmpty(item.CustomTheme) ? item.Theme : item.CustomTheme, new DialogParameters<string>()
            {
                Title = "重命名会话", TrapFocus = false
            });
        var data = await result.Result;
        if (data.Cancelled) return;

        item.CustomTheme = data.Data?.ToString();

        if (await AiChatSessionService.UpdateAsync(item))
            ToastService.ShowSuccess("更新成功");
        else
            ToastService.ShowError("更新失败");
    }

    private async Task DeleteSessionCommand(AiChatSessionEntity item)
    {
        var result = await DialogService.ShowWarningAsync("确定删除会话吗？", "警告", "确定");
        var data = await result.Result;
        if (data.Cancelled) return;

        if (await AiChatSessionService.DeleteAsync(x => x.Key == item.Key) is not null)
        {
            ToastService.ShowSuccess("删除成功");
            Content.Item1.Remove(item);
            if (Content.Item1.Any(x => !x.IsDefault) && Content.Item1.Count > 0)
            {
                var first = Content.Item1.First();
                first.IsDefault = true;
                selectedAiChatSessionEntity = first;
                await AiChatSessionService.UpdateAsync(first);
            }
        }
        else
        {
            ToastService.ShowError("删除失败");
        }
    }

    /// <summary>
    /// 添加新角色
    /// </summary>
    /// <returns></returns>
    private async Task AddNewOccupationCommand()
    {
        if (string.IsNullOrEmpty(newOccupationName))
        {
            ToastService.ShowWarning("角色名称不能为空");
            return;
        }

        if (string.IsNullOrWhiteSpace(newOccupationDescription))
        {
            ToastService.ShowWarning("角色描述不能为空");
            return;
        }

        if ((await CustomAssistantOccupationService.QueryAsync(x => x.Name == newOccupationName)).Any())
        {
            ToastService.ShowWarning("角色名称已存在");
            return;
        }

        var entity = new CustomAssistantOccupationEntity()
        {
            Name = newOccupationName,
            Description = newOccupationDescription
        };

        if (await CustomAssistantOccupationService.AddAsync(entity))
        {
            ToastService.ShowSuccess("添加成功");
            occupations = await GetAllOccupationsAsync();
            if (newOccupationIsDefault)
                selectedOccupationType =
                    occupations.FirstOrDefault(x => x.OccupationName == newOccupationName);
            newOccupationName = string.Empty;
            newOccupationDescription = string.Empty;
            newOccupationIsDefault = false;
        }
        else
        {
            ToastService.ShowError("添加失败");
        }
    }

    /// <summary>
    /// 删除自定义角色
    /// </summary>
    /// <returns></returns>
    private async Task DeleteCustomOccupationCommand()
    {
        if (selectedOccupationType is null)
            return;
        // 判断是否有回话使用了该角色
        var sessions = await AiChatSessionService.QueryAsync(x =>
            x.Occupation == AssistantOccupation.Custom &&
            x.CustomOccupationName == selectedOccupationType.OccupationName);

        if (sessions.Any())
        {
            var warningDialog = await DialogService.ShowConfirmationAsync("有会话使用了该角色，删除后会话将使用默认角色，是否继续删除？");
            var result = await warningDialog.Result;
            if (result.Cancelled)
                return;


            if (await DeleteCustomOccupationAsync())
            {
                foreach (var item in sessions) item.CustomOccupationName = selectedOccupationType.OccupationName;

                if (!await AiChatSessionService.UpdateRangeAsync(sessions)) ToastService.ShowError("更新会话失败");
            }
        }
        else
        {
            await DeleteCustomOccupationAsync();
        }
    }

    private async Task<bool> DeleteCustomOccupationAsync()
    {
        if (await CustomAssistantOccupationService.DeleteAsync(x =>
                x.Name == selectedOccupationType.OccupationName) is not null)
        {
            ToastService.ShowSuccess("删除成功");
            occupations = await GetAllOccupationsAsync();
            selectedOccupationType = occupations.FirstOrDefault();
            return true;
        }
        else
        {
            ToastService.ShowError("删除失败");
            return false;
        }
    }

    private void OccupationChangedCommand(OccupationType? obj)
    {
        selectedOccupationType = obj;
        selectOccupationVersion++;
        if (selectedAiChatSessionEntity is null)
            return;
        selectedAiChatSessionEntity.Occupation = obj?.Occupation ?? AssistantOccupation.Common;
        selectedAiChatSessionEntity.CustomOccupationName = obj?.Occupation == AssistantOccupation.Custom
            ? obj?.OccupationName
            : string.Empty;
    }


    private Task SaveAsync()
    {
        return Dialog.CloseAsync(Content);
    }

    private Task CancelAsync()
    {
        return Dialog.CancelAsync();
    }
}