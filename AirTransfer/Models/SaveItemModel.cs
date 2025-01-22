using TabbyCat.Shared.Enums;

using TabbyCat.Shared.Enums;

namespace AirTransfer.Models;

public partial class SaveItemModel
{
    public string Guid { get; set; } = String.Empty;


    public string? Title { get; set; }


    public string? Content { get; set; }


    public string? SourceDeviceName { get; set; }


    public string? Time { get; set; }


    public SendType SendType { get; set; }
}