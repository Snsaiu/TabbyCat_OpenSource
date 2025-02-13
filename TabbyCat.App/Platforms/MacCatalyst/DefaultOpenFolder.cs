using TabbyCat.App.Interfaces;

namespace TabbyCat.App;

public sealed class DefaultOpenFolder : IOpenFolder
{
    public void OpenFolder(string path)
    {
        System.Diagnostics.Process.Start("Open", path);
    }
}