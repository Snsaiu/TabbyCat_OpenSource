using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace TuDog.Extensions;

public static class HttpClientExtensions
{
    public static async Task DownloadFilePostAsync(this HttpClient client, string fileUrl, string savePath,
        Action<double>? progress = null)
    {
            using var content = new StringContent(string.Empty);
            using var response = await client.PostAsync(fileUrl, content);
            await DownImplAsync(response, savePath, progress);
    }

    private static async Task DownImplAsync(HttpResponseMessage response, string savePath,
        Action<double>? progress = null)
    {
        response.EnsureSuccessStatusCode(); // 确保请求成功

        // 获取文件总大小（如果服务器支持）
        var totalBytes = response.Content.Headers.ContentLength;

        using var contentStream = await response.Content.ReadAsStreamAsync();
        using var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

        var buffer = new byte[8192];
        long totalRead = 0;
        int bytesRead;

        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            await fileStream.WriteAsync(buffer, 0, bytesRead);
            totalRead += bytesRead;

            // 计算进度
            if (totalBytes.HasValue && progress != null)
            {
                var percentage = (double)totalRead / totalBytes.Value * 100;
                progress(percentage);
            }
        }
    }

    public static async Task<TResult?> PostRequestAsync<TParams, TResult>(this HttpClient client, string url,
        TParams request) where TParams : class
    {
        try
        {
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json);
            content.Headers.ContentType = new("application/json");
            var response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return !response.IsSuccessStatusCode
                ? default
                : JsonConvert.DeserializeObject<TResult>(await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            return default;
        }
    }
    
    public static async Task PostRequestAsync<TParams>(this HttpClient client, string url,
        TParams request) where TParams : class
    {
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json);
            content.Headers.ContentType = new("application/json");
            var response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
    }
    

    /// <summary>
    /// 异步下载文件，并保存到指定路径。
    /// </summary>
    /// <param name="client">HttpClient 实例</param>
    /// <param name="fileUrl">文件 URL</param>
    /// <param name="savePath">本地保存路径</param>
    /// <param name="progress">可选，下载进度回调（0-100%）</param>
    public static async Task<bool> DownloadFileAsync(this HttpClient client, string fileUrl, string savePath,
        Action<double>? progress = null, Action<string>? onError = null)
    {
        try
        {
            using var response = await client.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead);
            await DownImplAsync(response, savePath, progress);
            return true;
        }
        catch (Exception e)
        {
            onError?.Invoke(e.Message);
            // 写入日志
            return false;
        }

    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="client"></param>
    /// <param name="fileName"></param>
    /// <param name="uploadUrl"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public static async Task<string> UploadFile(this HttpClient client, string fileName, string uploadUrl)
    {
        if (!File.Exists(fileName))
            throw new FileNotFoundException("File not found.", fileName);

        using var form = new MultipartFormDataContent();
        using var fileStream = File.OpenRead(fileName);
        var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new("application/octet-stream");

        form.Add(streamContent, "file", Path.GetFileName(fileName));

        var response = await client.PostAsync(uploadUrl, form);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    public static async Task DownloadFileAsync(this HttpClient client, string url, string savePath)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be null or empty.", nameof(url));

        if (string.IsNullOrWhiteSpace(savePath))
            throw new ArgumentException("Save path cannot be null or empty.", nameof(savePath));

        try
        {
            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            var directory = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            await using var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await stream.CopyToAsync(fileStream);
        }
        catch (Exception ex)
        {
            throw new IOException($"Error downloading file from {url} to {savePath}", ex);
        }
    }
}