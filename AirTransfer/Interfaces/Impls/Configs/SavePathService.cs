using TabbyCat.Shared.Interfaces;

namespace AirTransfer.Interfaces.Impls.Configs
{
    public class SavePathService : ISavePathService
    {
        public string? GetPath()
        {
            return Preferences.Default.Get<string>("savePath", string.Empty);
        }

        public void SavePath(string path)
        {
            Preferences.Default.Set<string>("savePath", path);
        }
    }
}