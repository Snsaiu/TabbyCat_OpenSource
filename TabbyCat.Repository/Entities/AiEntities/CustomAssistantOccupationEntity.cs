using System.ComponentModel.DataAnnotations;

namespace TabbyCat.Repository.Entities.AiEntities;

public sealed class CustomAssistantOccupationEntity:UserBaseEntity
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;
}