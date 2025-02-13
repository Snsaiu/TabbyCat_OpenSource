using TabbyCat.App.Interfaces.Impls;
using TabbyCat.App.Interfaces.Impls.TcpTransfer;
using TabbyCat.App.Interfaces.Impls.Configs;
namespace TabbyCat.App;


public sealed class TcpLoopListenContent(FileSavePathBase fileSavePathBase) :TcpLoopListenContentBase(fileSavePathBase)
{

}