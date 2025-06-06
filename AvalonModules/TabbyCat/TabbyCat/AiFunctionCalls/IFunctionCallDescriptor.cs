using System.Threading;
using TabbyCat.AiFunctionCalls.Models;

namespace TabbyCat.AiFunctionCalls;

public interface IFunctionCallDescriptor
{
    string FunctionName { get; }
    string Description();
    Task<object?> QueryAsync(object? input, CancellationToken cancellationToken = default);
}

public interface IFunctionCallDescriptor<TInput> : IFunctionCallDescriptor
    where TInput : IParameterModel
{
    Task<OutputParameter> QueryDataAsync(TInput? input, CancellationToken cancellationToken = default);

    async Task<object?> IFunctionCallDescriptor.QueryAsync(object? input, CancellationToken cancellationToken)
    {
        var result = await QueryDataAsync(
            input is null ? default : JsonConvert.DeserializeObject<TInput>(input.ToString()!),
            cancellationToken);
        return result;

    }
}