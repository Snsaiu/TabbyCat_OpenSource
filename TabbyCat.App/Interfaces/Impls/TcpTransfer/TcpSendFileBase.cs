using System.Net.Sockets;
using TabbyCat.App.Models;

using TabbyCat.Shared.Interfaces;

namespace TabbyCat.App.Interfaces.Impls.TcpTransfer;

/// <summary>
///     tcp 发送文件基类
/// </summary>
public abstract class TcpSendFileBase : TcpSendBase<SendFileModel, ProgressValueModel>
{
    protected override SendMetadataMessage GetMetaDataMessage(SendFileModel message)
    {
        var fileInfo = new FileInfo(message.FileFullPath);
        var isCompress = message is ICompress;
        return new SendMetadataMessage(message.Flag, message.TargetFlag, Path.GetFileName(message.FileFullPath),
            fileInfo.Length, isCompress);
    }

    protected override async Task SendProcessAsync(NetworkStream sender, SendFileModel message,
        IProgress<ProgressValueModel>? progress, CancellationToken cancellationToken)
    {
        var fileInfo = new FileInfo(message.FileFullPath);

        var totalBytes = fileInfo.Length;
        var bytesSent = 0;

        var buffer = new byte[8192]; // 8KB 缓冲区
        int bytesRead;

        double lastProgress = -1;

        await using var fs = new FileStream(message.FileFullPath, FileMode.Open, FileAccess.Read);
        while ((bytesRead = await fs.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            await sender.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
            bytesSent += bytesRead;
            // 计算并显示上传进度
            var p = (double)bytesSent / totalBytes;
            if (Math.Abs(p - lastProgress) >= 0.1)
            {
                lastProgress = p;
                progress?.Report(new(message.Flag, message.TargetFlag, p));
            }
        }

        // 如果是压缩包，当上传结束后要删除
        if (message is ICompress) File.Delete(message.FileFullPath);
        if (lastProgress != 1)
            progress?.Report(new(message.Flag, message.TargetFlag, 1));
    }
}