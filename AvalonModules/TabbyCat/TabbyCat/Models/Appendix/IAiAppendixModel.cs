namespace TabbyCat.Models.Appendix;


/// <summary>
/// AI模型附件接口
/// </summary>
public interface IAiAppendixModel
{
    string Type { get; }

    object Data { get; set; }
}


/// <summary>
/// AI模型附件范型接口
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IAiAppendixModel<T> : IAiAppendixModel
{
    string Type { get; }

    T Data { get; set; }

    object IAiAppendixModel.Data
    {
        get => this.Data;
        set => this.Data = (T)value;
    }
}