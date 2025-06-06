namespace YouYan.Hamster.ResultModels
{
    public interface IResultModel
    {
        bool Ok { get; }

        string ErrorMsg { get; } 
        
        object Data { get;  }

        IResultModel Success();
        
        IResultModel Error(string errorMsg);
        
        IResultModel Success(object data);

    }

    public interface IResultModel<T> : IResultModel
    {
       new T Data { get; }
       
       object IResultModel.Data => (object)Data;
       
       new IResultModel<T> Success();
       IResultModel<T> Success(T data);
       new IResultModel<T> Error(string errorMsg);
       
       IResultModel IResultModel.Error(string errorMsg) => Error(errorMsg);

       IResultModel IResultModel.Success() => Success();

       IResultModel IResultModel.Success(object data) => Success((T)data);
    }
    
    
    
    
}