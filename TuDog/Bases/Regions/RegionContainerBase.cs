using Avalonia.Controls;

namespace TuDog.Bases.Regions;

public abstract class RegionContainerBase
{

    public abstract void TryAdd(string regionName, ContentControl region);

    public abstract void Remove(string regionName);

    public abstract ContentControl GetRegion(string regionName);
    
    public abstract bool Exists(string regionName);
    
    
}