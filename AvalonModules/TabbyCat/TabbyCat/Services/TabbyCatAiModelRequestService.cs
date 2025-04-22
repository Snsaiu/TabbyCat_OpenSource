using TabbyCat.Enums;
using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests;
using TabbyCat.Models.AiReqRes.AiChatRequests.OpenAi;
using TabbyCat.Models.AiReqRes.AiChatRequests.TabbyCatAi;
using TabbyCat.Models.AiReqRes.AiChatResponses;
using TabbyCat.Models.Appendix;
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

        foreach (var message in requestModel.Messages)
        {
            var newMessage = new TabbyCatMessageItem();
            newMessage.Role = message.Role;

            if (message.Appendixes.Any())
            {
                // 修改message中content属性的内容
                List<IAiAppendixModel> appendixModels = [];
                foreach (var item in message.Appendixes)
                    switch (item.AppendixType)
                    {
                        case AppendixType.Image:
                            appendixModels.Add(new ImageUrlAiAppendixModel()
                                { Data = new() { Url = $"data:image/png;base64,{item.Content}" } });
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
                newMessage.AppendixModels = appendixModels;
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
        return Task.FromResult(
            UnityResponseModel.StreamData(response.Choices.FirstOrDefault()?.Delta.Audio.Transcript ?? string.Empty));
    }
}