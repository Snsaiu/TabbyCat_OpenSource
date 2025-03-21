using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests;
using TabbyCat.Models.AiReqRes.AiChatResponses;
using TabbyCat.Shared.Interfaces;

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

    protected AiChatRequestServiceBase(TRequestModel requestModel, TAiModel aiModel)
    {
        _requestModel = requestModel;
        _aiModel = aiModel;
    }

    protected abstract Task<UnityResponseModel> ConvertResponseToUnityResponseModel(TResponseModel response);

    protected virtual HttpRequestMessage BuildHttpRequestMessage(TAiModel aiModel)
    {
        if (aiModel is IApiDomain apiDomain)
        {
            var urlPath = apiDomain.ApiDomain;

            if (aiModel is IApiPath apiPath) urlPath = $"{apiDomain.ApiDomain}{apiPath.ApiPath}";
            var request = new HttpRequestMessage(HttpMethod.Post, urlPath);

            request.Headers.Add("Accept", "application/json");

            if (apiDomain is IApiKey apiKey)
                request.Headers.Add("Authorization", $"Bearer {apiKey.ApiKey}");

            return request;
        }

        throw new NotImplementedException();
    }

    protected virtual Task<string> RequestModelToJsonString(TRequestModel requestModel)
    {
        return Task.FromResult(JsonConvert.SerializeObject(requestModel));
    }

    private async Task<HttpRequestMessage> FillHttpRequestMessage()
    {
        var requestMessage = BuildHttpRequestMessage(_aiModel);

        var json = await RequestModelToJsonString(_requestModel);

        var content = new StringContent(
            json,
            Encoding.UTF8, "application/json");

        requestMessage.Content = content;

        return requestMessage;
    }

    [Obsolete("建议使用sse服务")]
    public async Task<UnityResponseModel?> ProcessRequestAsync()
    {
        try
        {
            var requestMessage = await FillHttpRequestMessage();

            using var client = new HttpClient();
            using var response = await client.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode == false)
                return UnityResponseModel.Error($"请求失败:{response.ReasonPhrase}");

            response.EnsureSuccessStatusCode();

            var responseString = PreProcessResponse(await response.Content.ReadAsStringAsync());
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

            using var client = new HttpClient();
            using var response = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            if (response.IsSuccessStatusCode == false)
            {
                action?.Invoke(UnityResponseModel.Error(response.ReasonPhrase ?? string.Empty));
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
                        if (!string.IsNullOrWhiteSpace(line))
                            try
                            {
                                var responseString = PreProcessResponse(line);
                                var responseModel = JsonConvert.DeserializeObject<TResponseModel>(responseString);
                                if (responseModel is null)
                                {
                                    action?.Invoke(UnityResponseModel.Error($"数据解析错误:{line}"));

                                    return;
                                }
                                else
                                {
                                    var resultModel = await ConvertResponseToUnityResponseModel(responseModel);
                                    if (string.IsNullOrEmpty(resultModel?.Content))
                                        continue;

                                    var cancel = await action?.Invoke(resultModel);
                                    if (cancel == true)
                                        return;
                                }
                            }
                            catch (Exception e)
                            {
                                // 打印错误日志
                            }
                        }
                        catch (OperationCanceledException canceledException)
                        {
                            action?.Invoke(UnityResponseModel.Error("已取消聊天"));
                            return;
                        }
                    }

                    action?.Invoke(UnityResponseModel.Success());
                }, cancellationToken);
            }
        }
        catch (OperationCanceledException taskCanceledException)
        {
            return;
        }
        catch (Exception e)
        {
            action?.Invoke(UnityResponseModel.Error(e.Message));
        }
    }
}