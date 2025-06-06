using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Extensions;

namespace TabbyCat.Repository.Entities.AiEntities;

public partial class AiChatSessionEntity : UserBaseEntity
{

    /// <summary>
    /// 多轮对话的主题内容，默认可能是会话1 会话2 会话3......
    /// </summary>
    [NotifyPropertyChangedFor(nameof(Header))] [ObservableProperty]
    private string _theme = string.Empty;

    /// <summary>
    /// 自定义主题名称，如果为空，那么默认使用Theme，否则优先使用CustomTheme
    /// </summary>
    [NotifyPropertyChangedFor(nameof(Header))] [ObservableProperty]
    private string? _customTheme;


    [Ignore]
    public string Header => string.IsNullOrEmpty(CustomTheme)
        ? Theme.Replace("\n", "").Replace("\r", "")
        : CustomTheme.Replace("\n", "").Replace("\r", "");


    public AssistantOccupation Occupation { get; set; }

    /// <summary>
    /// 自定义角色,如果<see cref="AssistantOccupation"/>是<see cref="AssistantOccupation.Custom"/>，那么这个字段必须有值
    /// </summary>
    public string? CustomOccupationName { get; set; }

    /// <summary>
    /// 是否是默认的会话
    /// </summary>
    public bool  IsDefault { get; set; }

    public static AiChatSessionEntity CreateDefault(AssistantOccupation occupation=AssistantOccupation.Common)
    {
        return new AiChatSessionEntity { Theme = "默认会话", Occupation = occupation, IsDefault = true};
    }

    public static AiChatSessionEntity CreateCustom(string occupationName)
    {
        return new AiChatSessionEntity(){Theme = "默认会话",Occupation = AssistantOccupation.Custom,IsDefault = true,CustomOccupationName = occupationName};
    }

}