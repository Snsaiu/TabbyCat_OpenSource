using System.Collections.ObjectModel;
using FantasyResultModel;

namespace TabbyCat.Shared.Interfaces;

/// <summary>
/// 可设置别名
/// </summary>
public interface IAlias
{
    /// <summary>
    /// 别名
    /// </summary>
    string Alias { get; set; }
}

public interface IHasModels<T> : ISelectable<T>
{
    Task<IEnumerable<string>> GetModelsAsync();

    ObservableCollection<T> Models { get; set; }
}

public interface IInitializeable
{
    Task<ResultBase<bool>> InitializeAsync();
}

public interface ISelectable<T>
{
    T SelectedModel { get; set; }
}

/// <summary>
/// 显示调用
/// </summary>
public interface IHasCustomModel : IHasModels<string>
{
    /// <summary>
    /// 如果有自定义模型，则要显示调用
    /// </summary>
    /// <returns></returns>
    async Task<ObservableCollection<string>> GetAllModelsAsync()
    {
        var ms = await GetModelsAsync();
        return [.. ms, "Custom"];
    }

    string CustomModelName { get; set; }
}

public interface IApiKey
{
    string ApiKey { get; set; }
}

public interface IApiDomain
{
    string ApiDomain { get; set; }
}

public interface IApiPath
{
    string ApiPath { get; set; }
}

public interface ITopP
{
    double TopP { get; set; }
}

public interface IDeployName
{
    string DeployName { get; set; }
}

public interface IEndPoint
{
    string EndPoint { get; set; }
}

public interface ISaved
{
    bool IsSaved { get; }
}