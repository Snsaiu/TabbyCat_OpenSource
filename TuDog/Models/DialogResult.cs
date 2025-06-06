namespace TuDog.Models;

public class DialogResult(bool ok)
{
    public bool Ok { get; set; } = ok;
}

public class DialogResultData(bool ok, object? data) : DialogResult(ok)
{
    public object? Data { get; set; } = data;
}

public class DialogResultData<T>(bool ok, T? data) : DialogResult(ok)
{
    public T? Data { get; set; } = data;

    public static implicit operator DialogResultData<T>(DialogResultData data)
    {
        return data.Data is (T and var result) ? new DialogResultData<T>(true, result) : new(false, default);
    }
}