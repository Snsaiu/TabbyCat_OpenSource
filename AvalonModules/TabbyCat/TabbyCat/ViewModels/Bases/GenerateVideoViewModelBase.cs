using CommunityToolkit.Mvvm.ComponentModel;
using TabbyCat.Models.AiMediaResponses;

namespace TabbyCat.ViewModels.Bases;

/// <summary>
/// 通过提示词生成视频基类
/// </summary>
/// <typeparam name="TRequest">请求模型</typeparam>
/// <typeparam name="TPrompt">提示词模型</typeparam>
/// <typeparam name="TParameter">参数模型</typeparam>
public abstract partial class
    GenerateVideoViewModelBase<TRequest, TPrompt, TParameter> : AiMediaViewModelBase<TRequest, TPrompt, TParameter>
    where TRequest : AiMediaRequestModelBase<TPrompt, TParameter>
{
    protected override string DownloadFileExtension { get; } = ".mp4";

    /// <summary>
    /// 分辨率
    /// </summary>
    public virtual IEnumerable<string> Size { get; } =
        ["832*480", "480*832", "624*624", "1280*720", "720*1280", "960*960", "832*1088", "1088*832"];

    /// <summary>
    /// 选择的分辨率
    /// </summary>
    [ObservableProperty] private string _selectedSize = "1280*720";

    /// <summary>
    /// 提示词
    /// </summary>
    [ObservableProperty] private string _prompt = string.Empty;

    protected override Task<bool> ValidateConfirmAsync()
    {
        return Task.FromResult(!string.IsNullOrWhiteSpace(Prompt));
    }
}