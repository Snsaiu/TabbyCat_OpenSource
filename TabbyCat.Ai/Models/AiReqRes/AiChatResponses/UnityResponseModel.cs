namespace TabbyCat.Ai.Models.AiReqRes.AiChatResponses;

public class UnityResponseModel
{
    public string? Content { get; set; }

    /// <summary>
    /// 是否请求成功
    /// </summary>
    public bool Ok { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;

    private UnityResponseModel()
    {

    }


    public static UnityResponseModel Error(string errorMessage)
    {
        return new UnityResponseModel
        {
            Ok = false,
            ErrorMessage = errorMessage
        };
    }

    public static UnityResponseModel Success(string content)
    {
        return new UnityResponseModel
        {
            Ok = true,
            Content = content
        };
    }

}