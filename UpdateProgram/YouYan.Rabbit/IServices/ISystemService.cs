using YouYan.Rabbit.Extensions;

namespace YouYan.Rabbit.IServices
{
    public interface ISystemService
    {
        /// <summary>
        /// 系统是32位还是64位
        /// </summary>
        string SystemBit { get; }


        /// <summary>
        /// 获得系统架构
        /// </summary>
        string OSArchitecture { get; }

        /// <summary>
        /// 获得系统
        /// </summary>
        string OSPlatform { get; }

        AppOsType OsType { get; }
    }
}