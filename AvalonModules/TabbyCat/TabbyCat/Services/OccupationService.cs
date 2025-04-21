using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Service.AiServices;
using TabbyCat.Shared;
using TabbyCat.Shared.Enums;
using TuDog.IocAttribute;

namespace TabbyCat.Services;

[Register<IOccupationService>]
public sealed class OccupationService(IUser user,ICustomAssistantOccupationService customAssistantOccupationService):IOccupationService
{
    public async Task<IEnumerable<OccupationType>> GetAllOccupationsAsync()
    {
        var customOccupations = await customAssistantOccupationService.QueryAsync(x=> x.Email==user.Email);
        var temps = customOccupations.Select(item => new OccupationType(AssistantOccupation.Custom, item.Name)).ToList();

        if (user.LoginSuccess())
        {
            temps.AddRange(Enum.GetValues<AssistantOccupation>().Where(x => x != AssistantOccupation.Custom)
                .Select(item => new OccupationType(item, LocalizationResourceManager.Instance[item.ToString()])));
        }
        else
        {
            temps.Add(new OccupationType(AssistantOccupation.Common,LocalizationResourceManager.Instance[AssistantOccupation.Common.ToString()]));
        }
        return temps;
    }
}