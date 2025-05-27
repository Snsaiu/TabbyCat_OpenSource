using TabbyCat.Repository.Entities.AiEntities;

namespace TabbyCat.Models;

public sealed class UploadSettingsDto
{
    public string Email { get; set; } = string.Empty;
    public IEnumerable<AiTemplateSettingEntity> Settings { get; set; }
}