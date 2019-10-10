using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
 
    public class ServiceParameterSchema
    {
        public ServiceParameterSchema()
        {

        }
        internal ServiceParameterSchema(ParameterInfo info)
        {
            this.ParameterType = info.ParameterType;
            this.Name = info.Name;
        }
        /// <summary>
        /// 参数类型
        /// </summary>
        public Type ParameterType { get; set; }
        /// <summary>
        /// 参数名称
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// 参数的序列化器
        /// </summary>
        //public ISerializer Serializer { get; set; }
    }
}
