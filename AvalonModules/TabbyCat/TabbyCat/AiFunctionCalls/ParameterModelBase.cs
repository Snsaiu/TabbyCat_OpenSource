using TabbyCat.AiFunctionCalls.Models;

namespace TabbyCat.AiFunctionCalls;

public abstract class ParameterModelBase : IParameterModel
{
    protected virtual string ToJsonTemplate()
    {
        return JsonConvert.SerializeObject(this);
    }

    protected abstract string ParameterDescription();


    public string Description()
    {
        return $"Json格式如下:{ToJsonTemplate()},参数描述:{ParameterDescription()}";
    }
}