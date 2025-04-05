using System;
using TabbyCat.Repository.Entities.AiEntities;

namespace TabbyCat.Models;

public class PanelSettingModel
{
    public IEnumerable<AiChatSessionEntity> AllSessions { get; set; } = [];
    
    public AiApiModelBase? AiApiModel{ get; set; }
}