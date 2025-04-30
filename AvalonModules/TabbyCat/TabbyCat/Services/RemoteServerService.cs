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
    private readonly HttpClient _httpClient = factory.CreateClient(ConstParameter.Auth);

    public async Task<ResultBase<string>> GetAiKeyAsync()
    {
        var queryKeyResult = await _httpClient.GetAsync("/api/app/ai-api-key/api-key");
        if (!queryKeyResult.IsSuccessStatusCode)
            return new ErrorResultModel<string>(queryKeyResult.ReasonPhrase ?? "query api key error!");

        var key = await queryKeyResult.Content.ReadAsStringAsync();

        return new SuccessResultModel<string>(key);
    }

    /// <summary>
    /// 上传图片
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public async Task<ResultBase<string>> UploadImageAsync(string fileName)
    {
        try
        {
            var result = await _httpClient.UploadFile(fileName, "/api/app/ai-image-cache/upload-image");
            if (string.IsNullOrEmpty(result))
                return new ErrorResultModel<string>(AppResources.FileUploadError);
            return new SuccessResultModel<string>($"https://api.yyan.cc{result}");
        }
        catch (Exception e)
        {
            return new ErrorResultModel<string>(e.Message);
        }
    }
}