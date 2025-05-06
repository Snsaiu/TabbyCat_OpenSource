using Microsoft.Extensions.Logging;
using TabbyCat.Enums;
using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests;
using TabbyCat.Models.AiReqRes.AiChatRequests.OpenAi;
using TabbyCat.Models.AiReqRes.AiChatRequests.TabbyCatAi;
using TabbyCat.Models.AiReqRes.AiChatResponses;
using TabbyCat.Models.Appendix;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Extensions;
using TuDog.Bootstrap;

namespace TabbyCat.Services;

public sealed class TabbyCatAiModelRequestService(TabbyCatAiRequestModel requestModel, TabbyCatAiModel aiModel)
    : AiChatRequestServiceBase<TabbyCatAiRequestModel, TabbyCatAiModel, TabbyCatAiResponseModel>(requestModel, aiModel)
{

    protected override Task<string> RequestModelToJsonString(TabbyCatAiRequestModel requestModel)
    {
        // 用新的模型，不要修改入参的模型

        var newsModel = requestModel.ToJson().ToObject<TabbyCatAiRequestModel>();

        newsModel.Contents = [];

        var lastMessasge = requestModel.Messages.LastOrDefault();
        if (lastMessasge is not null)
        {
            if (lastMessasge.Appendixes.Any())
            {
                // 如果有附件，那么qwq-plus 不可用，需要切换成qwen-vl-max
                newsModel.Model = "qwen-vl-max";
                newsModel.EnableUseInternet = false;
                Logger.LogDebug("用户输入的消息中带有附件，使用模型{Model},并且关闭联网搜索功能。",newsModel.Model);
                // 删除system角色
                requestModel.Messages.RemoveAll(x => x.Role == Role.System);

            }
            else
            {
                if (newsModel.EnableUseInternet || newsModel.EnableDeepThinking)
                {
                    newsModel.Model = "qwq-plus";
                    Logger.LogDebug("用户输入的消息中没有附件，使用联网状态为:{EnableInternet};使用深度搜索状态为:{DeepThinking}；使用的模型为:{Model}",newsModel.EnableUseInternet,newsModel.EnableDeepThinking,newsModel.Model);
                }
                else
                {
                    Logger.LogDebug("用户没有使用联网功能和深度思考功能，使用的模型为{Model}。",newsModel.Model);
                }
            }
        }

        foreach (var message in requestModel.Messages)
        {
            var newMessage = new TabbyCatAiRequestModel.TabbyCatMessageItem
            {
                Role = message.Role
            };

            if (message.Appendixes.Any() && newsModel is { EnableUseInternet: false, EnableDeepThinking: false })
            {

                // 修改message中content属性的内容
                List<IAiAppendixModel> appendixModels = [];
                foreach (var item in message.Appendixes)
                    switch (item.AppendixType)
                    {
                        case AppendixType.Image:
                            appendixModels.Add(new ImageUrlAiAppendixModel()
                                { Data = new() { Url = item.Content } });
                            break;
                        case AppendixType.File:
                            throw new NotImplementedException();
                            break;
                        case AppendixType.Audio:
                            appendixModels.Add(new AudioAppendixModel() { Data = new() { Data = item.Content } });
                            break;
                        case AppendixType.Video:
                            throw new NotImplementedException();
                            break;
                        case AppendixType.Link:
                            throw new NotImplementedException();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                appendixModels.Add(new TextAppendixModel() { Data = message.Content });
                newMessage.Content = appendixModels;
            }
            else
            {
                newMessage.Content = message.Content;
            }

            newsModel.Contents.Add(newMessage);
        }

        return Task.FromResult<string>(JsonConvert.SerializeObject(newsModel));

    }

    protected override string PreProcessResponse(string responseString)
    {
        return responseString.Replace("data: ", "");
    }

    protected override Task<UnityResponseModel> ConvertResponseToUnityResponseModel(TabbyCatAiResponseModel response)
    {
        var delta = response.Choices.FirstOrDefault()?.Delta;
        if (delta.Audio is not null)
        {
            if (string.IsNullOrEmpty(delta.Audio.Transcript))
                return Task.FromResult(!string.IsNullOrEmpty(delta.ReasoningContent)
                    ? UnityResponseModel.StreamData(delta.ReasoningContent)
                    : UnityResponseModel.StreamData(delta.Content));
            return Task.FromResult(
                UnityResponseModel.StreamData(response.Choices.FirstOrDefault()?.Delta.Audio.Transcript ??
                                              string.Empty));
        }
        else
        {
            return Task.FromResult(!string.IsNullOrEmpty(delta.ReasoningContent)
                ? UnityResponseModel.StreamData(delta.ReasoningContent)
                : UnityResponseModel.StreamData(delta.Content));
        }
    }
}