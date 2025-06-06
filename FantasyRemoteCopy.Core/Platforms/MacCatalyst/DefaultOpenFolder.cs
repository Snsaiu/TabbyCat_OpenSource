namespace FantasyRemoteCopy.Core.Platforms;

public class DefaultOpenFolder:IOpenFolder
{
    public void OpenFolder(string path)
    {

        System.Diagnostics.Process.Start("Open", path);
    }
}