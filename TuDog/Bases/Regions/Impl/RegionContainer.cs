using Avalonia.Controls;

namespace TuDog.Bases.Regions.Impl;

public class RegionContainer:RegionContainerBase
{
    private Dictionary<string,ContentControl> _regions = new();
    
    public override void TryAdd(string regionName, ContentControl region) => _regions[regionName] = region;

    public override void Remove(string regionName) => _regions.Remove(regionName);

    public override ContentControl GetRegion(string regionName) => _regions.TryGetValue(regionName, out ContentControl region) ? region : throw new NullReferenceException($"Region {regionName} not found");

    public override bool Exists(string regionName) => _regions.ContainsKey(regionName);
}