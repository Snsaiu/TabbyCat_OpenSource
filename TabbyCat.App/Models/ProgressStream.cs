namespace TabbyCat.App.Models;

/// <summary>
/// 带有进度的<see cref="Stream"/>
/// </summary>
public class ProgressStream : Stream
{
    private readonly Stream stream;

    private long totalBytesRead = 0;

    private int lastReportedProgress = -1; // 上次触发 ProgressChanged 的进度

    public event Action<long, long> ProgressChanged;

    public ProgressStream(Stream stream)
    {
        this.stream = stream;
    }

    public override void Flush()
    {
        stream.Flush();
    }



    public override long Seek(long offset, SeekOrigin origin)
    {
        return stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        stream.SetLength(value);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        var bytesRead = stream.Read(buffer, offset, count);
        totalBytesRead += bytesRead;
        ReportProgress();
        return bytesRead;
    }

    private void ReportProgress()
    {
        var currentProgress = (int)((double)totalBytesRead / Length * 100);
        if (Math.Abs(currentProgress - lastReportedProgress) < 3) return;

        lastReportedProgress = currentProgress; // 更新上次触发进度
        ProgressChanged?.Invoke(totalBytesRead, Length);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        stream.Write(buffer, offset, count);
        totalBytesRead += count;
        ReportProgress();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            stream.Dispose();
        }

        base.Dispose(disposing);
    }

    public override long Length => stream.Length;
    public override bool CanRead => stream.CanRead;
    public override bool CanSeek => stream.CanSeek;
    public override bool CanWrite => stream.CanWrite;

    public override long Position
    {
        get => stream.Position;
        set => stream.Position = value;
    }
}