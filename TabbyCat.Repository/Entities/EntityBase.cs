using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using SQLite;
using TabbyCat.Shared.Extensions;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.Repository.Entities;

/// <summary>
/// 带有主键的实体基类
/// </summary>
public abstract class EntityBase : ObservableObject, IPrimaryKey<Guid>, IDeepClone
{
    [PrimaryKey]
    public Guid Key { get; set; } = Guid.NewGuid();

    public object DeepClone()
    {
        var json = JsonConvert.SerializeObject(this);
        var entity = JsonConvert.DeserializeObject(json, GetType());
        return entity ?? throw new NullReferenceException();
    }
}