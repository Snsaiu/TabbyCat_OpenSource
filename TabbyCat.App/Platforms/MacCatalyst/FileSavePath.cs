using TabbyCat.App.Interfaces;
using TabbyCat.App.Interfaces.Impls;

using TabbyCat.Shared.Interfaces;


namespace TabbyCat.App;

public sealed class FileSavePath(ISavePathService savePathService) : DeskTopFileSavePathBase(savePathService);