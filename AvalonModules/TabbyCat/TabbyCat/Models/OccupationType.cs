using TabbyCat.Shared.Enums;

namespace TabbyCat.Models;

public class OccupationType(AssistantOccupation occupation, string occupationName)
{
    public AssistantOccupation Occupation { get; init; } = occupation;
    public string OccupationName { get; init; } = occupationName;
}