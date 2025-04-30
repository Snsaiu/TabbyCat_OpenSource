using FantasyResultModel;

namespace TabbyCat.IServices;

public interface IRemoteServerService
{
    Task<ResultBase<string>> GetAiKeyAsync();

    /// <summary>
    /// 上传图片
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    Task<ResultBase<string>> UploadImageAsync(string fileName);

}