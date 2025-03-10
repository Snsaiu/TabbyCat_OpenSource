using System;
using System.Collections.Generic;
using System.Text;

namespace TuDog.IocAutoRegisterSourceGenerator.Models
{
    internal class DiscoverModel
    {
        public string? InterfaceFullName { get; set; }

        public string ImplementFullName { get; set; } = string.Empty;

        public LifeType LifeType { get; set; } = LifeType.Transient;    
    }

    internal enum LifeType
    {
        //
        // 摘要:
        //     Specifies that a single instance of the service will be created.
        Singleton,
        //
        // 摘要:
        //     Specifies that a new instance of the service will be created for each scope.
        //
        //
        // 言论：
        //     In ASP.NET Core applications a scope is created around each server request.
        Scoped,
        //
        // 摘要:
        //     Specifies that a new instance of the service will be created every time it is
        //     requested.
        Transient
    }
}
