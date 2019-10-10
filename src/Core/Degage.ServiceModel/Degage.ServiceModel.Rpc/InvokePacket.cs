using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    public class InvokePacket : BasePacket
    {
        /// <summary>
        /// 调用的 Id
        /// </summary>
        public String Id { get; set; }
        /// <summary>
        /// 调用接口的名称
        /// </summary>
        public String InterfaceName { get; set; }

        /// <summary>
        /// 调用操作的名称
        /// </summary>
        public String OperationName { get; set; }

        /// <summary>
        /// 调用的参数
        /// </summary>
        public Object[] ParameterValues { get; set; }

        /// <summary>
        /// 调用参数的类型信息程序集限定名称的字符串数组
        /// </summary>
        public String[] ParameterTypes { get; set; }


        public DateTime InvokeTime { get; set; }
    }
}
