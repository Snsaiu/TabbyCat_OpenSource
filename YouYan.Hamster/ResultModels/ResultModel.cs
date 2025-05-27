namespace YouYan.Hamster.ResultModels;

internal sealed class ResultModel:IResultModel
{
    public bool Ok { get; private set; }
    public string ErrorMsg { get; private set; }
    public object Data { get; private set; }
    public IResultModel Success()
    {
        return new ResultModel()
        {
            Ok = true
        };
    }

    public IResultModel Error(string errorMsg)
    {
        return new ResultModel()
        {
            Ok = false,
            ErrorMsg = errorMsg
        };
    }

    public IResultModel Success(object data)
    {
        return new ResultModel()
        {
            Ok = true,
            Data = data
        };
    }
}

internal class ResultModel<T> : IResultModel<T>
{
    public bool Ok { get;private set; }
    public string ErrorMsg { get;private set; }
    public T Data { get;private set; }

    public IResultModel<T> Success()
    {
        return new ResultModel<T>()
        {
            Ok = true
        };
    }

    public IResultModel<T> Error(string errorMsg)
    {
        return new ResultModel<T>()
        {
            Ok = false,
            ErrorMsg = errorMsg
        };
    }

    public IResultModel<T> Success(T data)
    {
        return new ResultModel<T>()
        {
            Ok = true,
            Data = data
        };
    }
}