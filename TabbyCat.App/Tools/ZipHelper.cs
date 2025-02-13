using System.IO.Compression;

namespace TabbyCat.App.Tools;

public static class ZipHelper
{
    public static void CreateZipFromFolder(string folderPath, string zipFilePath)
    {
        // 删除现有的ZIP文件
        if (File.Exists(zipFilePath)) File.Delete(zipFilePath);
        // 创建新的ZIP文件
        ZipFile.CreateFromDirectory(folderPath, zipFilePath, CompressionLevel.Fastest, true);
    }

    /// <summary>
    /// 解压zip文件到指定文件夹
    /// </summary>
    /// <param name="zipFilePath">zip文件</param>
    /// <param name="directoryPath">解压路径</param>
    public static void ExtractToDirectory(string zipFilePath, string directoryPath) => ZipFile.ExtractToDirectory(zipFilePath, directoryPath);
}