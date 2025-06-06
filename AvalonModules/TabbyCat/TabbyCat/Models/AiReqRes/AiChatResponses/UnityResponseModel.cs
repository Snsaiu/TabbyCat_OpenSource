namespace TabbyCat.Models.AiReqRes.AiChatResponses;

public class UnityResponseModel
{
    public string? Content { get; set; }

    /// <summary>
    /// 是否请求成功
    /// </summary>
    public bool Ok { get; set; }

    public bool StreamFinished { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;

    private UnityResponseModel()
    {
    }


    public static UnityResponseModel Error(string errorMessage)
    {
        return new()
        {
            Ok = false,
            ErrorMessage = errorMessage
        };
    }

    public static UnityResponseModel Success(string content)
    {
        return new()
        {
            Ok = true,
            Content = content,
            StreamFinished = true
        };
    }

    public static UnityResponseModel StreamData(string content)
    {
        return new()
        {
            Ok = true,
            Content = content,
            StreamFinished = false
        };
    }

    public static UnityResponseModel Success()
    {
        return new()
        {
            Ok = true,
            StreamFinished = true
        };
    }
}