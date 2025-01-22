using Newtonsoft.Json;
using SQLite;
using TabbyCat.Shared.Interfaces;

namespace TabbyCat.Repository.Entities;

/// <summary>
/// 带有主键的实体基类
/// </summary>
public abstract class EntityBase : IPrimaryKey<Guid>, IDeepClone,IConvertible
{
    [PrimaryKey]
    public Guid Key { get; set; } = Guid.NewGuid();

    public object DeepClone()
    {
        var json = JsonConvert.SerializeObject(this);
        var entity = JsonConvert.DeserializeObject(json, GetType());
        return entity ?? throw new NullReferenceException();
    }

    public TypeCode GetTypeCode()
    {
        return TypeCode.Object;
    }

    public bool ToBoolean(IFormatProvider? provider)
    {
       return false;
    }

    public byte ToByte(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public char ToChar(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public DateTime ToDateTime(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public decimal ToDecimal(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public double ToDouble(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public short ToInt16(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public int ToInt32(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public long ToInt64(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public sbyte ToSByte(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public float ToSingle(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public string ToString(IFormatProvider? provider)
    {
        return this.Key.ToString();
    }

    public object ToType(Type conversionType, IFormatProvider? provider)
    {
        return Convert.ChangeType(this, conversionType, provider);
    }

    public ushort ToUInt16(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public uint ToUInt32(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public ulong ToUInt64(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }
}