using TabbyCat.Enums;

namespace TabbyCat.IServices;

public interface IBackgroundImageConfig
{
    BackgroundImageStatus Status { get; set; }
    string? CustomImage { get; set; }

    double Opacity { get; set; }
}