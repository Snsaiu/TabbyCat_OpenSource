namespace TuDog.Extensions;

public static class HttpClientExtensions
{
    /// <summary>
    /// 异步下载文件，并保存到指定路径。
    /// </summary>
    /// <param name="client">HttpClient 实例</param>
    /// <param name="fileUrl">文件 URL</param>
    /// <param name="savePath">本地保存路径</param>
    /// <param name="progress">可选，下载进度回调（0-100%）</param>
    public static async Task DownloadFileAsync(this HttpClient client, string fileUrl, string savePath,
        Action<double>? progress = null)
    {
        using var response = await client.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead);
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