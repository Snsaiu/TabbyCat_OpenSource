using TabbyCat.App.Interfaces;
using TabbyCat.App.Tools;

using TabbyCat.Shared.Interfaces;

namespace TabbyCat.App.Models;

public class SendCompressFileModel : SendFileModel, ICompress
{
    public SendCompressFileModel(SendFolderModel folderModel) : this(folderModel.Flag, folderModel.TargetFlag,
        folderModel.FileFullPath, folderModel.Port)
    {
    }


    /// <summary>
    /// 针对多个文件，将多个文件压缩成zip
    /// </summary>
    /// <param name="flag">本地flag</param>
    /// <param name="targetFlag">目标flag</param>
    /// <param name="files">需要被压缩的文件集合</param>
    public SendCompressFileModel(string flag, string targetFlag, IEnumerable<string> files, int port) : base(flag,
        targetFlag, "", port)
    {
        // 创建一个随机的文件夹名称,这个文件夹用于将files 拷贝到该目录，用于后期的压缩
        var tempName = Guid.NewGuid().ToString("N");
        var tempFolder = Path.Combine(FileSystem.CacheDirectory, tempName);
        Directory.CreateDirectory(tempFolder);
        foreach (var file in files)
        {
            if (!File.Exists(file))
                continue;

            File.Copy(file, Path.Combine(tempFolder, Path.GetFileName(file)));
        }

        ZipFolder(tempFolder);

        Directory.Delete(tempFolder, true);
    }

    public SendCompressFileModel(string flag, string targetFlag, string fileFullPath, int port) : base(flag, targetFlag,
        fileFullPath, port)
    {
        if (!Directory.Exists(FileFullPath))
            throw new DirectoryNotFoundException();
        ZipFolder(fileFullPath);
    }

    private void ZipFolder(string fileFullPath)
    {
        var zipName = Path.GetFileName(fileFullPath) + ".zip";
        var compressPath = Path.Combine(FileSystem.CacheDirectory, zipName);

        ZipHelper.CreateZipFromFolder(fileFullPath, compressPath);
        FileFullPath = compressPath;
    }
}