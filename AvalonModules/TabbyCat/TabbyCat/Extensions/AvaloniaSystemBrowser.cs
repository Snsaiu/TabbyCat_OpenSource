using System.Diagnostics;
using System.Net;
using System.Threading;
using Duende.IdentityModel.OidcClient.Browser;

namespace TabbyCat.Extensions;

public class AvaloniaSystemBrowser : IBrowser
{
    private readonly int _port;
   

    public AvaloniaSystemBrowser(int port)
    {
        _port = port;
      
    }

    public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
    {
        var listener = new HttpListener();
        listener.Prefixes.Add($"http://localhost:{_port}/");
        listener.Start();

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = options.StartUrl,
                UseShellExecute = true
            });
            
            var context = await listener.GetContextAsync(); // 等待回调
            var response = context.Response;
            string responseString = "Authentication successful. You can close this window.";
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
            response.Close();

            return new BrowserResult
            {
                Response = context.Request.Url?.ToString(),
                ResultType = BrowserResultType.Success
            };
        }
        catch (Exception ex)
        {
            return new BrowserResult { ResultType = BrowserResultType.UnknownError, Error = ex.Message };
        }
        finally
        {
            listener.Stop();
        }
    }
}