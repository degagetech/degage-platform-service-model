using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    /// <summary>
    /// 服务操作相关参数的序列化器类型说明
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class SerializerAttribute : System.Attribute
    {
        public Type SerializerType { get; private set; }
        public SerializerAttribute(Type serializerType) : base()
        {
            this.SerializerType = serializerType;
        }
        public SerializerAttribute(String typeString) : base()
        {
            this.SerializerType = Type.GetType(typeString);
            if (this.SerializerType == null)
            {
                throw ExceptionCode.NotFoundParameterSerializerType.NewException();
            }
        }
    }
}
