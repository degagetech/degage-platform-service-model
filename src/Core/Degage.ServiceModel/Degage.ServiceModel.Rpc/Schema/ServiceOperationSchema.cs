using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    /// <summary>
    /// 服务操作结构信息
    /// </summary>
    public class ServiceOperationSchema
    {
        public String Name { get; set; }

        internal MethodInfo MethodInfo { get; set; }
    }
}
