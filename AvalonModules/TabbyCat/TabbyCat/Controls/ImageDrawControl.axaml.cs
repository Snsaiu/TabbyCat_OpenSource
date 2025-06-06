using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TabbyCat.Controls;

public partial class ImageDrawControl : UserControl
{
    public enum  CanvasOperation
    {
       
        ReadyPen,
        Pen,
        ReadyMove,
        Move
    }
    
    public static readonly StyledProperty<string> BackgroundImageProperty = AvaloniaProperty.Register<ImageDrawControl, string>(
        nameof(BackgroundImage));

    
    public string BackgroundImage
    {
        get => GetValue(BackgroundImageProperty);
        set => SetValue(BackgroundImageProperty, value);
    }
    
    private WriteableBitmap _bitmap;
    private  int CanvasWidth = 600;
    private  int CanvasHeight = 600;

    private int _r = 0;
    private int _g = 0;
    private int _b = 0;

    private int _a = 255;

   private CanvasOperation _operation = CanvasOperation.Pen;
    
    private int _brushSize = 10;
    
  
    private Slider _penSizeSlider = null;
    
    private double _canvasScale = 1;
    
    private TransformGroup _transformGroup=new TransformGroup();
    private ScaleTransform _scaleTransform=new ScaleTransform();
    private TranslateTransform _moveTransforms=new TranslateTransform();
    
    public ImageDrawControl()
    {
        InitializeComponent();
        
        _penSizeSlider = this.FindControl<Slider>("penSizeSlider");
        _penSizeSlider.ValueChanged += (_, e) =>
        {
            _brushSize = (int)_penSizeSlider.Value;
        };
        
        this.Loaded += (_, _) =>
        {
            this.GetObservable(BackgroundImageProperty).Subscribe(x =>
            {
                if (!string.IsNullOrEmpty(x) && File.Exists(x))
                {
                    var bitmap = new Bitmap(x);
                    Background.Source = bitmap;
                    CanvasWidth = bitmap.PixelSize.Width;
                    CanvasHeight = bitmap.PixelSize.Height;
                    Background.Width = CanvasWidth;
                    Background.Height = CanvasHeight;
                    CanvasImage.Width= CanvasWidth;
                    CanvasImage.Height = CanvasHeight;
                 
                }
                else
                {
                  
                }

                _bitmap = new WriteableBitmap(
                    new PixelSize(CanvasWidth, CanvasHeight),
                    new Vector(96, 96),
                    Avalonia.Platform.PixelFormat.Bgra8888,
                    AlphaFormat.Premul
                );
                
                ClearCanvas(); // 可选：填充白色背景
                CanvasImage.Source = _bitmap;
            });
            _transformGroup.Children.Add(_scaleTransform);
            _transformGroup.Children.Add(_moveTransforms);
            this.imageContainer.RenderTransform = _transformGroup;

        };

    }
    
    private void SetEraser(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _operation = CanvasOperation.ReadyPen;
        this.imageContainer.Cursor = Avalonia.Input.Cursor.Default;
        (_r, _g, _b, _a) = (0, 0, 0, 0);
    }

    private void SetPen(object? sender,Avalonia.Interactivity.RoutedEventArgs e)
    {
        _operation = CanvasOperation.ReadyPen;
        this.imageContainer.Cursor = Avalonia.Input.Cursor.Default;
        (_r, _g, _b, _a) = (0, 0, 0, 255);
    }


    private void ClearCanvasClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => ClearCanvas();


    private void SetPixel(int centerX, int centerY, byte r, byte g, byte b,byte a)
    {
        using var fb = _bitmap.Lock();
        unsafe
        {
            var ptr = (byte*)fb.Address.ToPointer();
            var stride = fb.RowBytes;

            var radius = _brushSize / 2;
            var radiusSquared = radius * radius;

            for (var y = centerY - radius; y <= centerY + radius; y++)
            {
                for (var x = centerX - radius; x <= centerX + radius; x++)
                {
                    var dx = x - centerX;
                    var dy = y - centerY;

                    if (dx * dx + dy * dy > radiusSquared) continue; // 圆形判断！
                    if (x < 0 || x >= CanvasWidth || y < 0 || y >= CanvasHeight) continue;
                    var offset = y * stride + x * 4;
                    ptr[offset + 0] = b;
                    ptr[offset + 1] = g;
                    ptr[offset + 2] = r;
                    ptr[offset + 3] = a;
                }
            }
        }
    }
    
    private PixelPoint? _lastPoint = null; // 记录上一次位置
    
    private void DrawLine(PixelPoint p1, PixelPoint p2, byte r, byte g, byte b, byte a)
    {
        var dx = Math.Abs(p2.X - p1.X);
        var dy = Math.Abs(p2.Y - p1.Y);

        var sx = p1.X < p2.X ? 1 : -1;
        var sy = p1.Y < p2.Y ? 1 : -1;
        var err = dx - dy;

        var x = p1.X;
        var y = p1.Y;

        while (true)
        {
            SetPixel(x, y, r, g, b,a);

            if (x == p2.X && y == p2.Y)
                break;

            var e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x += sx;
            }

            if (e2 >= dx) continue;
            err += dx;
            y += sy;
        }
    }

    
    private void OnPointerDraw(PointerEventArgs e)
    {
        if (e.GetCurrentPoint(this.CanvasImage).Properties.IsLeftButtonPressed)
        {
            var point = e.GetPosition(CanvasImage);
            var current = new PixelPoint((int)point.X, (int)point.Y);

            if (_lastPoint != null)
            {
                DrawLine(_lastPoint.Value, current, (byte)_r, (byte)_g, (byte)_b,(byte)_a); // 黑色
            }
            else
            {
                SetPixel(current.X, current.Y, (byte)_r, (byte)_g, (byte)_b,(byte)_a);
            }

            _lastPoint = current;

            CanvasImage.InvalidateVisual();
        }
        else
        {
            _lastPoint = null; // 松开鼠标，清空
        }
    }
    
    private void ClearCanvas()
    {
        using var fb = _bitmap.Lock();
        unsafe
        {
            byte* ptr = (byte*)fb.Address.ToPointer();
            for (int i = 0; i < fb.RowBytes * CanvasHeight; i += 4)
            {
                ptr[i + 0] = 0; // B
                ptr[i + 1] = 0; // G
                ptr[i + 2] = 0; // R
                ptr[i + 3] = 0; // A
            }
        }
        CanvasImage.InvalidateVisual();
    }

    private void CanvasImage_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
       if(_operation == CanvasOperation.ReadyPen)
           _operation = CanvasOperation.Pen;
       else if (_operation == CanvasOperation.ReadyMove)
       {
           _operation = CanvasOperation.Move;
           var point = e.GetPosition(null);
           _lastPoint = new PixelPoint((int)point.X, (int)point.Y);
       }
    }

    private void CanvasImage_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_operation == CanvasOperation.Pen)
            OnPointerDraw(e);
        else if (_operation == (CanvasOperation.Move))
        {
            var position = e.GetPosition(null);
            var delta = position - new Point( _lastPoint.Value.X, _lastPoint.Value.Y );
            _lastPoint = new PixelPoint((int)position.X, (int)position.Y);
            _moveTransforms.X+=delta.X;
            _moveTransforms.Y+=delta.Y;
        }
    }

    private void CanvasImage_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_operation == CanvasOperation.Pen)
        {
            _operation = CanvasOperation.ReadyPen;
            _lastPoint = null;
        }
        else if (_operation == CanvasOperation.Move)
        {
            _operation = CanvasOperation.ReadyMove;
            _lastPoint = null;
        }
    }
    
    public async Task SaveWithWhiteBackgroundAsync(string path)
    {
        // 创建一个和你的画布大小一样的 RenderTargetBitmap
        var composedBitmap = new RenderTargetBitmap(new PixelSize(CanvasWidth, CanvasHeight));

        using (var ctx = composedBitmap.CreateDrawingContext())
        {
            // 1. 先画白色背景
            ctx.FillRectangle(Brushes.White, new Rect(0, 0, CanvasWidth, CanvasHeight));

            // 2. 再画你的原始 bitmap 内容
            ctx.DrawImage(_bitmap, new Rect(0, 0, _bitmap.PixelSize.Width, _bitmap.PixelSize.Height), new Rect(0, 0, CanvasWidth, CanvasHeight));
        }
        
        if (path != null)
        {
            await using var stream = File.Create(path);
            composedBitmap.Save(stream);
        }
    }

    private void Enlarge(object? sender, RoutedEventArgs e)
    {
       if(_canvasScale>=1)
           return;

       _canvasScale += 0.1;
       ScaleCanvas();
    }


    private void ZoomOut(object? sender, RoutedEventArgs e)
    {
       if(_canvasScale<=0.1)
           return;
       _canvasScale -= 0.1;
       ScaleCanvas();
    }

  
    private void ScaleCanvas()
    {

        this._scaleTransform.ScaleX=_canvasScale;
        this._scaleTransform.ScaleY=_canvasScale;
        imageContainer.RenderTransform = _transformGroup;
        imageContainer.RenderTransformOrigin = new RelativePoint(0.5, 0.5,RelativeUnit.Relative);
       
    }

    private void SetMove(object? sender, RoutedEventArgs e)
    {
       this._operation = CanvasOperation.ReadyMove;
       this.imageContainer.Cursor = new Cursor(StandardCursorType.Hand);
    }
}