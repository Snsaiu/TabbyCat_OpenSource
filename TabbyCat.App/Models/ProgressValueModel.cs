using TabbyCat.App.Interfaces;

using TabbyCat.Shared.Interfaces;


namespace TabbyCat.App.Models;

public class ProgressValueModel(string flag, string targetFlag, double progress) : IProgressValue, IFlag, ITargetFlag
{
    public double Progress { get; } = progress;
    public string Flag { get; } = flag;
    public string TargetFlag { get; } = targetFlag;
}