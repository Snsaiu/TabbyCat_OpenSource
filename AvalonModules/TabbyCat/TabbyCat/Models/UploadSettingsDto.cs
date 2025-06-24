using TabbyCat.Repository.Entities.AiEntities;

namespace TabbyCat.Models;

public sealed class UploadSettingsDto
{
    public string Email { get; set; } = string.Empty;
    public IEnumerable<AiTemplateSettingEntity> Settings { get; set; }
}

public sealed class UploadDto<T>
{
    public required string Email { get; set; } 
    
    public required T Data { get; set; }
}