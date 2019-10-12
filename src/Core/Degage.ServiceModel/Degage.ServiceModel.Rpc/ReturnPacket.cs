using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    public class ReturnPacket :BasePacket
    {

        /// <summary>
        /// 调用 Id
        /// </summary>
        public String InvokeId { get; set; }

        /// <summary>
        /// 表示所调用操作执行成功与否
        /// </summary>
        public Boolean Success { get; set; }

        /// <summary>
        /// 调用的返回值
        /// </summary>
        public Object Content { get; set; }

        /// <summary>
        /// 调用的返回值的类型的程序集限定名
        /// </summary>
        public String ContentType { get; set; }
        /// <summary>
        /// 附加消息
        /// </summary>
        public String Message { get; set; }

        /// <summary>
        /// 状态码
        /// </summary>
        public Int32 Status { get; set; }

        /// <summary>
        /// 状态码的描述
        /// </summary>
        public String StatusDesc { get; set; }

        /// <summary>
        /// 表示服务器开始处理此请求的时间
        /// </summary>
        public DateTime HandleTime { get; set; }

        /// <summary>
        /// 服务器处理与此返回包关联的请求花费的时间，单位：ms
        /// </summary>
        public Int32 Elapsed { get; set; }
    }
}
