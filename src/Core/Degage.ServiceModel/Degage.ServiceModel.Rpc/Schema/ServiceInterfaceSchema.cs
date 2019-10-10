using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    /// <summary>
    /// 服务接口结构信息
    /// </summary>
    public class ServiceInterfaceSchema
    {
        /// <summary>
        /// 服务接口的名称
        /// </summary>
        public String Name { get; set; }
        internal Type InterfaceType { get; set; }
    }
}
