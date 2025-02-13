using System.Net.Sockets;
using TabbyCat.Shared.Enums;
using TabbyCat.App.Interfaces.Impls.Configs;
using TabbyCat.App.Models;
using TabbyCat.App.Tools;

namespace TabbyCat.App.Interfaces.Impls.TcpTransfer;

public abstract class TcpLoopListenContentBase(FileSavePathBase fileSavePathBase)
    : TcpLoopListenerBase<TransformResultModel<string>, ProgressValueModel, string>
{
    protected readonly FileSavePathBase FileSavePathBase = fileSavePathBase;

    protected override async Task HandleReceiveAsync(NetworkStream stream, SendMetadataMessage message,
        Action<TransformResultModel<string>> receivedCallBack,
        IProgress<ProgressValueModel>? progress, int port, CancellationToken cancellationToken)
    {
        if (message.SendType == SendType.Text)
        {
            var text = await ReceiveStringAsync(stream, message.Size);
            TransformResultModel<string> result = new(message.Flag, SendType.Text, text, port);
            receivedCallBack.Invoke(result);
        }
        else
        {
            var buffer = new byte[8192]; // 8KB 缓冲区
            var fileSize = message.Size;
            var guidFolder = Guid.NewGuid().ToString("N");

            Directory.CreateDirectory(Path.Combine(FileSavePathBase.SaveLocation, guidFolder));

            var saveFullPath = Path.Combine(FileSavePathBase.SaveLocation, guidFolder, message.Name);
            long receivedBytes = 0;

            await using var fs = new FileStream(saveFullPath, FileMode.Create, FileAccess.Write);
            int bytesRead;

            while ((bytesRead = await stream.ReadAsync(buffer, cancellationToken)) > 0)
            {
                await fs.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                receivedBytes += bytesRead;

                // 计算并显示下载进度
                var p = (double)receivedBytes / fileSize;
                var pModel = new ProgressValueModel(message.Flag, message.TargetFlag, p);
                progress?.Report(pModel);
            }

            fs?.Close();
            fs?.Dispose();

            //如果是需要解压的，那么要先解压，在删除
            if (message.IsCompress)
            {
                ZipHelper.ExtractToDirectory(saveFullPath, Path.Combine(FileSavePathBase.SaveLocation, guidFolder));
                File.Delete(saveFullPath);

                var fileName = Path.Combine(FileSavePathBase.SaveLocation, guidFolder,
                    Path.GetFileNameWithoutExtension(saveFullPath));
                TransformResultModel<string> result = new(message.Flag, SendType.Folder, fileName, port);
                receivedCallBack.Invoke(result);
            }
            else
            {
                TransformResultModel<string> result = new(message.Flag, SendType.File, saveFullPath, port);
                receivedCallBack.Invoke(result);
            }
        }
    }
}