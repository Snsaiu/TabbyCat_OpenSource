using Microsoft.AspNetCore.Components;
using TabbyCat.Ai.Models;
using TabbyCat.Service.AiServices;
using TabbyCat.Shared;
using TabbyCat.Shared.Enums;
using TabbyCat.VisualBase.Bases;

namespace TabbyCat.Ai.Bases;

public abstract class AiComponentBase:PageComponentBase
{
    protected override string GetModuleName() => "TabbyCat.Ai";
    
    [Inject] protected ICustomAssistantOccupationService CustomAssistantOccupationService { get; set; } = null!;
   
    protected async Task<IEnumerable<OccupationType>>GetAllOccupationsAsync()
    {
        var customOccupations = await this.CustomAssistantOccupationService.QueryAsync();
        var occupations = customOccupations.Select(item => new OccupationType(AssistantOccupation.Custom, item.Name)).ToList();
      
        occupations.AddRange(Enum.GetValues<AssistantOccupation>().Where(x=>x!=AssistantOccupation.Custom).Select(item => new OccupationType(item, LocalizationResourceManager.Instance[item.ToString()])));

        return occupations;
    }
}