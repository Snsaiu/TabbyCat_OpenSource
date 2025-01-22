namespace TabbyCat.Shared.Interfaces;


public interface IDeepClone
{
    object DeepClone();
}

public interface IDeepClone<out T> : IDeepClone where T : class
{
    T DeepClone();
    object IDeepClone.DeepClone()
    {
        return DeepClone();
    }
}