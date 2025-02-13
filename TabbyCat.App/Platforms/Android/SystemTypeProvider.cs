using TabbyCat.Shared.Enums;
using TabbyCat.App.Interfaces;

using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.App;

public sealed class SystemTypeProvider : ISystemType
{
    public SystemType System { get; } = SystemType.Android;
}