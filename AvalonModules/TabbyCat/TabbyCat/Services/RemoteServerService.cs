using System.IO;
using System.Net.Http;
using FantasyResultModel;
using FantasyResultModel.Impls;
using TabbyCat.Extensions;
using TabbyCat.IServices;
using TabbyCat.Shared.Languages;
using TuDog.Extensions;
using TuDog.IocAttribute;

namespace TabbyCat.Services;

[Register<IRemoteServerService>]
public sealed class RemoteServerService(IHttpClientFactory factory) : IRemoteServerService
{
    private readonly HttpClient _httpClient = factory.CreateClient();

    public Task<ResultBase<string>> GetAiKeyAsync()
    {
        // 当前图片生成默认使用的阿里云的模型
        throw new NotImplementedException("请在这里实现自己的获得AI Key的方式");
    }

    /// <summary>
    /// 上传图片
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns>返回图片上传后的下载URl地址</returns>
    public Task<ResultBase<string>> UploadImageAsync(string fileName)
    {
        throw new NotImplementedException("请在这里实现自己上传图片的逻辑的方式");
    }
}