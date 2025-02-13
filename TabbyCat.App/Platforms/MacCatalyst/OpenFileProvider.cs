using TabbyCat.App.Interfaces;

namespace TabbyCat.App;

public class OpenFileProvider : IOpenFileable
{
    public void OpenFile(string filename)
    {
        System.Diagnostics.Process.Start("open", filename);
    }
}