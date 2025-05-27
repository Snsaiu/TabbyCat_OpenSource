namespace YouYan.Hamster.ResultModels;

public static class ResultModelFactory
{
    public static IResultModel Success()
    {
        var result = new ResultModel();
        return result.Success();
    }

    public static IResultModel Error(string errorMsg)
    {
        var result = new ResultModel();
        return result.Error(errorMsg);
    }

    public static IResultModel Success(object data)
    {
        var result = new ResultModel();
        return result.Success(data);
    }
    
    public static IResultModel<T> Success<T>()
    {
        var result = new ResultModel<T>();
        return result.Success();
    }

    public static IResultModel<T> Error<T>(string errorMsg)
    {
        var result = new ResultModel<T>();
        return result.Error(errorMsg);
    }

    public static IResultModel<T> Success<T>(T data)
    {
        var result = new ResultModel<T>();
        return result.Success(data);
    }
}