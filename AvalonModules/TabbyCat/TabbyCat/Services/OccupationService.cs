using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Service.AiServices;
using TabbyCat.Shared;
using TabbyCat.Shared.Enums;
using TuDog.IocAttribute;

namespace TabbyCat.Services;

[Register<IOccupationService>]
public sealed class OccupationService(ICustomAssistantOccupationService customAssistantOccupationService)
    : IOccupationService
{
    public async Task<IEnumerable<OccupationType>> GetAllOccupationsAsync()
    {
        var customOccupations = await customAssistantOccupationService.QueryAsync();
        var temps = customOccupations.Select(item => new OccupationType(AssistantOccupation.Custom, item.Name))
            .ToList();
        
        temps.AddRange(Enum.GetValues<AssistantOccupation>().Where(x => x != AssistantOccupation.Custom)
            .Select(item => new OccupationType(item, LocalizationResourceManager.Instance[item.ToString()])));
        
        return temps;
    }
}