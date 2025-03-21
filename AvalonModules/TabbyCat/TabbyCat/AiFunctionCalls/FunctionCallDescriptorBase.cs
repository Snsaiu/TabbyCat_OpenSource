using System.Threading;
using TabbyCat.AiFunctionCalls.Models;

namespace TabbyCat.AiFunctionCalls;

public abstract class FunctionCallDescriptorBase<TInput> : IFunctionCallDescriptor<TInput>
    where TInput : IParameterModel, new()
{
    public abstract string FunctionName { get; }

    protected abstract string FunctionDescription();

    public string Description()
    {
        var input = new TInput();

        var description =
            $"{FunctionName.ToString()}的功能:{FunctionDescription()},输入参数描述:{input.Description()}。";
        return description;
    }

    public abstract Task<OutputParameter> QueryDataAsync(TInput? input, CancellationToken cancellationToken = default);
}