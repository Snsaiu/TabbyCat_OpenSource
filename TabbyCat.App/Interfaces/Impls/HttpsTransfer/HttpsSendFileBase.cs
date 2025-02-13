using TabbyCat.App.Models;

namespace TabbyCat.App.Interfaces.Impls.HttpsTransfer;

public abstract class HttpsSendFileBase()
    : HttpsSendBase<SendFileModel, ProgressValueModel>
{
    protected override async Task SendProcessAsync(HttpClient sender, SendFileModel message,
        IProgress<ProgressValueModel>? progress,
        CancellationToken cancellationToken)
    {
        var url = $"https://{message.TargetFlag}:{SendPort}/file";

        using (var fileStream = new FileStream(message.FileFullPath, FileMode.Open, FileAccess.Read))
        {
            var progressStream = new ProgressStream(fileStream);
            progressStream.ProgressChanged += (current, all) =>
            {
                progress?.Report(new ProgressValueModel(message.Flag, message.TargetFlag, current / all));
            };

            using (var content = new MultipartFormDataContent())
            {
                var filecontent = new StreamContent(progressStream);
                content.Add(filecontent, "file", Path.GetFileName(message.FileFullPath));
                var response = await sender.PostAsync(url, content, cancellationToken);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}