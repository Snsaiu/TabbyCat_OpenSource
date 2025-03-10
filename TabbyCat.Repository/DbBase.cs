using SQLite;

using TabbyCat.Shared.Extensions;

using Xamarin.Essentials;

namespace TabbyCat.Repository;

public abstract class DbBase
{
    protected readonly string dbPath;
    protected readonly SQLiteAsyncConnection connection;

    public DbBase()
    {
        if (!OperatingSystem.IsAndroid() && !OperatingSystem.IsIOS())
        {
            dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "tabbycat.db");
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

        }
        else
        {
            dbPath = Path.Combine(FileSystem.AppDataDirectory, "tabbycat.db");
        }

        connection = new SQLiteAsyncConnection(dbPath);

        // ReSharper disable once VirtualMemberCallInConstructor
        CreateTableAsync().WaitTask(null, x => throw x);
    }

    /// <summary>
    /// 创建表
    /// </summary>
    protected abstract Task CreateTableAsync();
}