//using System.Net;
//using System.Text;
//using FantasyRemoteCopy.UI.Enums;
//using FantasyRemoteCopy.UI.Extensions;
//using FantasyRemoteCopy.UI.Interfaces.Impls.Configs;
//using FantasyRemoteCopy.UI.Interfaces.Impls.TcpTransfer;
//using FantasyRemoteCopy.UI.Models;
//using FantasyRemoteCopy.UI.Tools;

//namespace FantasyRemoteCopy.UI.Interfaces.Impls.HttpsTransfer;

//public class HttpsLoopListenContent(FileSavePathBase fileSavePathBase)
//    : LoopListenerBase<TransformResultModel<string>, ProgressValueModel, string>
//{
//    protected readonly FileSavePathBase FileSavePathBase = fileSavePathBase;

//    public string WatchIp { get; set; } = string.Empty;

//    public int Port { get; set; }

//    /// <summary>
//    ///     用户名称，用作密钥key
//    /// </summary>
//    public string UserName { get; set; } = string.Empty;

//    public SendType ReceiveType { get; set; }

//    public override async Task ReceiveAsync(Action<TransformResultModel<string>> receivedCallBack,
//        IProgress<ProgressValueModel>? progress, CancellationToken cancellationToken)
//    {
//        if (string.IsNullOrEmpty(WatchIp))
//            throw new NullReferenceException($"监听的IP {nameof(WatchIp)} 不能为空");
//        if (string.IsNullOrEmpty(UserName))
//            throw new NullReferenceException($"{nameof(UserName)} 不能为空");

//        var listenner = new HttpListener();

//        if (ReceiveType == SendType.Text)
//        {
//            listenner.Prefixes.Add($"http://{WatchIp}:{Port}/text/");
//            listenner.Start();

//            var context = await listenner.GetContextAsync();
//            var request = context.Request;
//            var receivedSignature = request.Headers["X-Signature"];
//            var response = context.Response;
//            if (request.HttpMethod == "POST")
//                using (var reader = new StreamReader(request.InputStream, Encoding.UTF8))
//                {
//                    var requestData = await reader.ReadToEndAsync();
//                    // 检查密钥
//                    if (CertificateGenerator.VerifyHMACSignatureForMessage(requestData, receivedSignature,
//                            UserName))
//                        context.Response.StatusCode = (int)HttpStatusCode.OK;

//                    var model = requestData.ToObject<SendTextModel>();
//                    receivedCallBack?.Invoke(
//                        new TransformResultModel<string>(model.Flag, SendType.Text, model.Text));
//                }

//            listenner.Close();
//            listenner = null;
//        }

//        if (ReceiveType == SendType.File)
//        {
//        }
//        // 文件夹
//    }
//}