namespace TabbyCat.Shared.Interfaces;

public interface IPrimaryKey<T>
{
    T Key { get; }
}