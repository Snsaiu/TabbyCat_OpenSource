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
            var exeName = Path.GetFileName(Environment.ProcessPath).Split(".").FirstOrDefault();
            if (exeName is null)
                throw new NullReferenceException("无法获得程序文件名称");

            var folder = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), exeName);

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            dbPath = Path.Combine(folder, "tabbycat.db");
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