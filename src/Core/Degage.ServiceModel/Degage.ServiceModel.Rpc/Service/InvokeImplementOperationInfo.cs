using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    /// <summary>
    /// 包含实现操作调用的相关信息
    /// </summary>
    public class InvokeImplementOperationInfo : InvokeOperationInfo
    {
        /// <summary>
        /// 此调用操作对应的接口的实现类的结构信息
        /// </summary>
        public ServiceImplementSchema ImplementSchema { get; set; }

        public static InvokeImplementOperationInfo Create(Type interfaceType, MethodInfo operationInfo, Type implementType)
        {
            InvokeImplementOperationInfo info = new InvokeImplementOperationInfo();

            Fill(info, interfaceType, operationInfo);

            var implementSchema = ServiceModelSchema.GetServiceImplementSchema(implementType);
            info.ImplementSchema = implementSchema;

            return info;
        }
    }
}
