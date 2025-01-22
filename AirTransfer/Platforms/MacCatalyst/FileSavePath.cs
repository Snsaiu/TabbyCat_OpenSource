using AirTransfer.Interfaces;
using AirTransfer.Interfaces.Impls;

using TabbyCat.Shared.Interfaces;


namespace AirTransfer;

public sealed class FileSavePath(ISavePathService savePathService) : DeskTopFileSavePathBase(savePathService);