using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using LibVLCSharp.Avalonia;
using LibVLCSharp.Shared;

namespace TabbyCat.Controls;

public partial class VideoControl : UserControl, IDisposable
{
    private readonly LibVLC _libVlc = new();
    private MediaPlayer _mediaPlayer;
    private VideoView _videoView;
    private Image _image;

    private Button _playButton;
    private Button _stopButton;

    public static readonly StyledProperty<string> UrlProperty = AvaloniaProperty.Register<VideoControl, string>(
        nameof(Url));

    public static readonly StyledProperty<string> ThumbnailProperty = AvaloniaProperty.Register<VideoControl, string>(
        nameof(Thumbnail));

    public static readonly StyledProperty<bool> AutoPlayProperty = AvaloniaProperty.Register<VideoControl, bool>(
        nameof(AutoPlay));

    public static readonly StyledProperty<int> ButtonSizeProperty = AvaloniaProperty.Register<VideoControl, int>(
        nameof(ButtonSize));

    public int ButtonSize
    {
        get => GetValue(ButtonSizeProperty);
        set => SetValue(ButtonSizeProperty, value);
    }

    public bool AutoPlay
    {
        get => GetValue(AutoPlayProperty);
        set => SetValue(AutoPlayProperty, value);
    }

    public string Thumbnail
    {
        get => GetValue(ThumbnailProperty);
        set => SetValue(ThumbnailProperty, value);
    }

    public string Url
    {
        get => GetValue(UrlProperty);
        set => SetValue(UrlProperty, value);
    }


    public VideoControl()
    {
        InitializeComponent();

        _videoView = this.FindControl<VideoView>("videoView");
        _image = this.FindControl<Image>("thumbnail");
        _playButton = this.FindControl<Button>("playButton");
        _stopButton = this.FindControl<Button>("stopButton");
        _mediaPlayer = new(_libVlc);
        _videoView.MediaPlayer = _mediaPlayer;
        Loaded += (_, _) =>
        {
            this.GetObservable(UrlProperty).Subscribe(x =>
            {
                if (x is null)
                {
                    if (!string.IsNullOrEmpty(Thumbnail) && File.Exists(Thumbnail) && !AutoPlay)
                    {
                        _videoView.IsVisible = false;
                        _image.IsVisible = true;
                    }

                    else
                    {
                        _image.IsVisible = false;
                        _videoView.IsVisible = true;
                    }
                }
                else
                {
                    if (AutoPlay && File.Exists(x))
                    {
                        _image.IsVisible = false;
                        _videoView.IsVisible = true;
                        using var media = new Media(_libVlc, x);
                        _mediaPlayer.Play(media);
                        return;
                    }

                    _image.IsVisible = true;
                    _videoView.IsVisible = false;
                }

            });

            this.GetObservable(ThumbnailProperty).Subscribe(x =>
            {
                if (_image is null)
                    return;

                if (string.IsNullOrEmpty(x) || !File.Exists(x))
                    return;
                _image.Source = new Bitmap(x);
            });

            this.GetObservable(ButtonSizeProperty).Subscribe(x =>
            {
                if (x == 0)
                    return;
                _stopButton.FontSize = x;
                _playButton.FontSize = x;
            });

            this.GetObservable(AutoPlayProperty).Subscribe(x =>
            {
                if (x && !string.IsNullOrEmpty(Url) && File.Exists(Url))
                {
                    _image.IsVisible = false;
                    _videoView.IsVisible = true;
                }
            });
        };

        _mediaPlayer.Stopped += (_, _) =>
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                if (!string.IsNullOrEmpty(Thumbnail) && File.Exists(Thumbnail))
                {
                    _image.IsVisible = true;
                    _videoView.IsVisible = false;
                }
                else
                {
                    _image.IsVisible = false;
                    _videoView.IsVisible = true;
                }
            });
        };

    }
    private void Play(object? sender, RoutedEventArgs e)
    {
        _image.IsVisible = false;
        _videoView.IsVisible = true;
        using var media = new Media(_libVlc, Url);
        _mediaPlayer.Play(media);
    }

    private void Stop(object? sender, RoutedEventArgs e)
    {
        _mediaPlayer.Stop();
        if (!string.IsNullOrEmpty(Thumbnail) && File.Exists(Thumbnail))
        {
            _image.IsVisible = true;
            _videoView.IsVisible = false;
        }
        else
        {
            _image.IsVisible = false;
            _videoView.IsVisible = true;
        }

    }


    public void Dispose()
    {
        _libVlc.Dispose();
        _mediaPlayer.Dispose();
    }



}