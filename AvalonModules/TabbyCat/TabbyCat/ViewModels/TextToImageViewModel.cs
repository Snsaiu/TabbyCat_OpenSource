using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TabbyCat.Models.RunningHubs;
using TabbyCat.Shared.Enums;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public partial class TextToImageViewModel : AiMediaViewModelBase
{
    protected override RunningHubWorkType RunningHubWorkType { get; } = RunningHubWorkType.TextToImage;
    protected override long WorkFlowId { get; } = 1896880845263675393;

    [ObservableProperty] private string imageDescription = string.Empty;

    [ObservableProperty] private ObservableCollection<string> imageSizes =
    [
        "1:1 (Perfect Square)", "2:3 (Classic Portrait)", "3:4 (Golden Ratio)", "3:5 (Elegant Vertical)",
        "4:5 (Artistic Frame)", "5:7 (Balanced Portrait)", "5:8 (Tall Portrait)", "7:9 (Modern Portrait)",
        "9:16 (Slim Vertical)", "9:19 (Tall Slim)", "9:21 (Ultra Tall)", "9:32 (Skyline)", "3:2 (Golden Landscape)",
        "4:3 (Classic Landscape)", "5:3 (Wide Horizon)", "5:4 (Balanced Frame)", "7:5 (Elegant Landscape)",
        "8:5 (Cinematic View)", "9:7 (Artful Horizon)", "16:9 (Panorama)", "19:9 (Cinematic Ultrawide)"
    ];

    [ObservableProperty] private string selectImageSize = "3:4 (Golden Ratio)";

    protected override Task<bool> ValidateConfirmAsync()
    {
        return Task.FromResult(!string.IsNullOrEmpty(ImageDescription));
    }

    protected override async Task<bool> OnConfirmAsync()
    {
        var ratioNode = new NodeInfoListItem()
            { NodeId = "52", FieldName = "aspect_ratio", FieldValue = SelectImageSize };
        var txtNode = new NodeInfoListItem() { NodeId = "50", FieldName = "text", FieldValue = ImageDescription };
        return await PublishTaskAsync([txtNode, ratioNode]);
    }
}