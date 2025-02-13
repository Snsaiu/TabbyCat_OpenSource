namespace TabbyCat.App.Models;

public class SendFolderModel(string flag, string targetFlag, string fullPath, int port)
    : SendModelBase(flag, targetFlag, fullPath, port)
{
    public override long Size => throw new NotImplementedException();
}