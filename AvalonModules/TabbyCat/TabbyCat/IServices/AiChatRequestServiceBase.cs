using System.Net.Http;
using System.Text;
using System.Threading;
using FantasyResultModel;
using FantasyResultModel.Impls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests;
using TabbyCat.Models.AiReqRes.AiChatResponses;
using TabbyCat.Shared.Interfaces;
using TabbyCat.Shared.Languages;
using TuDog.Bootstrap;

namespace TabbyCat.IServices;

public abstract class
    AiChatRequestServiceBase<TRequestModel, TAiModel, TResponseModel> : IAiResponse<TRequestModel, TAiModel,
    TResponseModel>
    where TRequestModel : MessageSessionBase
    where TAiModel : AiApiModelBase
    where TResponseModel : ChatResponseBase
{
    protected readonly TRequestModel _requestModel;
    protected readonly TAiModel _aiModel;

    protected ILoggerFactory LoggerFactory = TuDogApplication.ServiceProvider.GetRequiredService<ILoggerFactory>();
    protected ILogger Logger { get; private set; }

    private HttpClient _httpClient;
    protected AiChatRequestServiceBase(TRequestModel requestModel, TAiModel aiModel)
    {
        var factory = TuDogApplication.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        _httpClient = factory.CreateClient();
        Logger = LoggerFactory.CreateLogger(nameof(AiChatRequestServiceBase<TRequestModel, TAiModel, TResponseModel>));

        _requestModel = requestModel;
        _aiModel = aiModel;
    }

    protected abstract Task<UnityResponseModel> ConvertResponseToUnityResponseModel(TResponseModel response);

    protected virtual ResultBase<HttpRequestMessage> BuildHttpRequestMessage(TAiModel aiModel)
    {
        if (aiModel is IApiDomain apiDomain)
        {
            var urlPath = apiDomain.ApiDomain;

            if (aiModel is IApiPath apiPath) urlPath = $"{apiDomain.ApiDomain}{apiPath.ApiPath}";
            var request = new HttpRequestMessage(HttpMethod.Post, urlPath);

            request.Headers.Add("Accept", "application/json");

            if (apiDomain is IApiKey apiKey)
                request.Headers.Add("Authorization", $"Bearer {apiKey.ApiKey}");

            return new SuccessResultModel<HttpRequestMessage>( request);
        }

        this.Logger.LogError("暂不支持提供者{0}不是{1}类型的AiModel。",aiModel.Provider.ToString(),typeof(IApiDomain));

        return new ErrorResultModel<HttpRequestMessage>(string.Format(AppResources.NotSupport,aiModel.Provider));
    }

    protected virtual Task<string> RequestModelToJsonString(TRequestModel requestModel)
    {
        return Task.FromResult(JsonConvert.SerializeObject(requestModel));
    }

    private async Task<ResultBase<HttpRequestMessage>> FillHttpRequestMessage()
    {
        var requestMessageResult = BuildHttpRequestMessage(_aiModel);
        if(!requestMessageResult.Ok)
            return new ErrorResultModel<HttpRequestMessage>(requestMessageResult.ErrorMsg??string.Format(AppResources.UnableConstructObject,typeof(HttpRequestMessage)));

        var json = await RequestModelToJsonString(_requestModel);

        var content = new StringContent(
            json,
            Encoding.UTF8, "application/json");

        Logger.LogDebug("聊天请求的json数据:{0}", json);

        requestMessageResult.Data.Content = content;

        return requestMessageResult;
    }

    [Obsolete("建议使用sse服务")]
    public async Task<UnityResponseModel?> ProcessRequestAsync()
    {
        try
        {
            var requestMessage = await FillHttpRequestMessage();
            if(!requestMessage.Ok)
                return UnityResponseModel.Error(requestMessage.ErrorMsg??string.Empty);

            using var client = new HttpClient();
            using var response = await client.SendAsync(requestMessage.Data);

            if (response.IsSuccessStatusCode == false)
                return UnityResponseModel.Error($"请求失败:{response.ReasonPhrase}");

            response.EnsureSuccessStatusCode();

            var responseString = PreProcessResponse(await response.Content.ReadAsStringAsync());
            if(string.IsNullOrEmpty(responseString))
                return UnityResponseModel.Success();
            var responseModel = JsonConvert.DeserializeObject<TResponseModel>(responseString);

            return responseModel == null ? null : await ConvertResponseToUnityResponseModel(responseModel);
        }
        catch (Exception e)
        {
            // todo记录到日志中
            return UnityResponseModel.Error(e.Message);
        }
    }


    protected virtual string PreProcessResponse(string responseString)
    {
        return responseString;
    }

    public async Task StreamProcessResponseAsync(Func<UnityResponseModel, Task<bool>> action,
        CancellationToken cancellationToken)
    {
        try
        {
            var requestMessage = await FillHttpRequestMessage();
            if (!requestMessage.Ok)
            {
                await action.Invoke(UnityResponseModel.Error(requestMessage.ErrorMsg??string.Empty));
                return;
            }


            using var response = await _httpClient.SendAsync(requestMessage.Data, HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            if (response.IsSuccessStatusCode == false)
            {
                Logger.LogError("数据解析错误:{0}",response.ReasonPhrase);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await action.Invoke(UnityResponseModel.Error(AppResources.MustLoginToUseTabbyCatAi));
                }
                else
                {
                    await action.Invoke(UnityResponseModel.Error(response.ReasonPhrase ?? string.Empty));
                }

                return;
            }

            response.EnsureSuccessStatusCode();
            using (var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken))
            using (var reader = new System.IO.StreamReader(responseStream))
            {
                await Task.Run(async () =>
                {
                    while (!reader.EndOfStream)
                    {
                        try
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            var line = await reader.ReadLineAsync(cancellationToken);
                            cancellationToken.ThrowIfCancellationRequested();

                            Logger.LogDebug("聊天返回数据:{0}", line);

                        if (!string.IsNullOrWhiteSpace(line))
                            try
                            {
                                var responseString = PreProcessResponse(line);
                                if(string.IsNullOrEmpty(responseString))
                                    return;
                                var responseModel = JsonConvert.DeserializeObject<TResponseModel>(responseString);
                                if (responseModel is null)
                                {
                                    await action.Invoke(UnityResponseModel.Error($"数据解析错误:{line}"));
                                    return;
                                }
                                else
                                {
                                    var resultModel = await ConvertResponseToUnityResponseModel(responseModel);
                                    if (string.IsNullOrEmpty(resultModel.Content))
                                        continue;

                                    var cancel = await action.Invoke(resultModel);
                                    if (cancel)
                                        return;
                                }
                            }
                            catch (Exception e)
                            {
                                Logger.LogError(e,e.Message);

                            }
                        }
                        catch (OperationCanceledException)
                        {
                            await action.Invoke(UnityResponseModel.Error("已取消聊天"));
                            return;
                        }
                    }

                    await action.Invoke(UnityResponseModel.Success());
                }, cancellationToken);
            }
        }
        catch (OperationCanceledException taskCanceledException)
        {
            Logger.LogWarning(taskCanceledException,"接收聊天内容取消。");

        }
        catch (Exception e)
        {
            Logger.LogError(e,e.Message);
            await action.Invoke(UnityResponseModel.Error(e.Message));
        }
    }
}