using TabbyCat.App.Interfaces.Impls.Configs;

using TabbyCat.Shared.Interfaces;

namespace TabbyCat.App.Interfaces.Impls;

public abstract class DeskTopFileSavePathBase : FileSavePathBase, IChangePathable
{
    private readonly ISavePathService savePathService;

    public DeskTopFileSavePathBase(ISavePathService savePathService)
    {
        this.savePathService = savePathService;
    }

    public override string SaveLocation
    {
        get
        {
            var path = savePathService.GetPath();
            return string.IsNullOrEmpty(path) ? base.SaveLocation : path;
        }
    }

    void IChangePathable.ChangedPath(string path)
    {
        savePathService.SavePath(path);
    }
}