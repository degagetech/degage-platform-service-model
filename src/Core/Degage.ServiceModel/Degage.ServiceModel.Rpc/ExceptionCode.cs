using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    internal sealed class ExceptionCode
    {
        /// <summary>
        /// 发射缓存添加失败
        /// </summary>
        public static Int32 EmitMetadataCacheAddFailed = 0x0001;

        /// <summary>
        /// 未能加载服务模型的结构信息
        /// </summary>
        public static Int32 UnloadServiceSchema = 0x2000;

        /// <summary>
        /// 未标识为服务接口
        /// </summary>
        public static Int32 UnidentifiedServiceInterfaceOnInterfaceType = 0x2001;

        /// <summary>
        /// 未标识为服务操作
        /// </summary>
        public static Int32 UnidentifiedServiceOperationOnMethod = 0x2002;

        /// <summary>
        /// 服务接口的类型与服务实现的类型不匹配
        /// </summary>
        public static Int32 NoMatchServiceInterfaceTypeWithImplementType = 0x3000;

        /// <summary>
        /// 服务操作路径相同
        /// </summary>
        public static Int32 ServiceOperationPathEqual = 0x3001;

    
        /// <summary>
        /// 序列化失败
        /// </summary>
        public static Int32 SerializationException = 0x4001;

        /// <summary>
        /// 服务通信失败
        /// </summary>
        public static Int32 ServiceCommunicationExceotion = 0x5001;

        /// <summary>
        /// 未能找到参数序列化器的类型
        /// </summary>
        public static Int32 NotFoundParameterSerializerType = 0x6001;
    }
}
