using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    public class ServiceImplementSchema
    {
        /// <summary>
        /// 服务实现实例的创建模式
        /// </summary>
        public InstantiateMode InstantiateMode { get; set; } = InstantiateMode.Singleton;
        internal Type ImplementType { get; set; }
    }
    /// <summary>
    /// 实例模式
    /// </summary>
    public enum InstantiateMode
    {
        Singleton=0,
        EachCall
    }

}
