using TabbyCat.App.Interfaces.Impls.Configs;
using TabbyCat.App.Interfaces.Impls.TcpTransfer;

namespace TabbyCat.App;

public sealed class TcpLoopListenContent(FileSavePathBase fileSavePathBase) : TcpLoopListenContentBase(fileSavePathBase)
{
}