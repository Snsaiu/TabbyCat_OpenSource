using FantasyResultModel;
using TabbyCat.Models;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;
using YouYan.Hamster.ResultModels;

namespace TabbyCat.IServices;

public interface IAiTemplateSettingSyncService
{
    /// <summary>
    /// 根据邮箱同步AI模型
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <returns>返回同步数据</returns>
    Task<IResultModel<IEnumerable<AiTemplateSettingEntity>>> SyncRemoteAiTemplateSettingEntitiesAsync(DownloadSettingDto downloadSettingDto);
    
    /// <summary>
    /// 查询最新的版本
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    Task<IResultModel< int>> QueryLatestVersionAsync(string email);
    
    /// <summary>
    /// 上传最新的版本
    /// </summary>
    /// <param name="email"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    Task UploadNewVersionAsync(string email,IEnumerable<AiTemplateSettingEntity> settings);
}