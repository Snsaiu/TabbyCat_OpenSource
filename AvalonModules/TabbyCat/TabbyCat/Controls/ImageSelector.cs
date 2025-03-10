using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;

using TabbyCat.Shared.Languages;

namespace TabbyCat.Controls;

public class ImageSelector : Button
{
    public static readonly StyledProperty<string> ImagePathProperty = AvaloniaProperty.Register<ImageSelector, string>(
        nameof(ImagePath), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<string> HeaderProperty = AvaloniaProperty.Register<ImageSelector, string>(
        nameof(Header));

    public static readonly StyledProperty<bool> ShowHeaderProperty = AvaloniaProperty.Register<ImageSelector, bool>(
        nameof(ShowHeader));

    public bool ShowHeader
    {
        get => GetValue(ShowHeaderProperty);
        set => SetValue(ShowHeaderProperty, value);
    }

    public string Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public string ImagePath
    {
        get => GetValue(ImagePathProperty);
        set => SetValue(ImagePathProperty, value);
    }

    public ImageSelector()
    {

        this.Click += async (s, e) =>
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel is null)
                throw new NullReferenceException();

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = AppResources.ChooseImage,
                AllowMultiple = false,
                FileTypeFilter =
                    [FilePickerFileTypes.ImageAll]
            });
            if (!files.Any())
                return;

            if (image is null)
                throw new NullReferenceException();

            var path = files.First().Path.LocalPath;
            image.Source = new Bitmap(path);
            ImagePath = path;
            removeButton!.IsVisible = true;
        };
    }

    private Button removeButton;
    private Image image;


    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        image = e.NameScope.Find<Image>("PART_image") ?? throw new NullReferenceException();
        removeButton = e.NameScope.Find<Button>("PART_removeBtn") ?? throw new NullReferenceException();

        removeButton.Click += (s, e) =>
        {
            ImagePath = string.Empty;
            image.Source = null;
            removeButton.IsVisible = false;
            e.Handled = true;
        };
    }
}