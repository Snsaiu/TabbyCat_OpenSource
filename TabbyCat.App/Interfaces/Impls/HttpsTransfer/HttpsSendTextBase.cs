using System.Text;

using TabbyCat.App.Models;

using Newtonsoft.Json;

namespace TabbyCat.App.Interfaces.Impls.HttpsTransfer;

public abstract class HttpsSendTextBase : HttpsSendBase<SendTextModel, ProgressValueModel>
{
    protected override async Task SendProcessAsync(HttpClient sender, SendTextModel message,
        IProgress<ProgressValueModel>? progress,
        CancellationToken cancellationToken)
    {
        // 获得对方的ip
        var url = $"http://{message.TargetFlag}:{SendPort}/text";

        var content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");

        // 发送请求
        var response = await sender.PostAsync(url, content, default);
        response.EnsureSuccessStatusCode();
        progress?.Report(new ProgressValueModel(message.Flag, message.TargetFlag, 1));
        sender.Dispose();
        sender = null;
    }
}