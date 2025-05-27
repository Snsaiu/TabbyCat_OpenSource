namespace TabbyCat.IServices;

public interface IAiTemplateSettingSyncManager
{
    Task StartLoopAsync();
    
    Task SyncSetting();
}