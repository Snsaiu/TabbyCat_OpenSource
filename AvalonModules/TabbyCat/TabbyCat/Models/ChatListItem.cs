using CommunityToolkit.Mvvm.ComponentModel;
using TuDog.Bootstrap;

namespace TabbyCat.Models;

public partial class ChatListItem : ModelBase
{
    [ObservableProperty] private Guid iD = Guid.Empty;
    [ObservableProperty] private string title = string.Empty;
    [ObservableProperty] private string content = string.Empty;
}