using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    public class TransferSetting
    {
        /// <summary>
        /// 传输地址
        /// </summary>
        public String Address { get; set; }

        /// <summary>
        /// 是否对传输的信息做加密处理
        /// </summary>
        public Boolean IsEncrypted { get; set; }

        /// <summary>
        /// 传输的密钥
        /// </summary>
        public String SecretKey { get; set; }

        /// <summary>
        /// 数据传输的超时时间
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// 打开超时设置
        /// </summary>
        public TimeSpan OpenTimeout { get; set; }

        /// <summary>
        /// 是否使用压缩传输的信息
        /// </summary>
        public Boolean IsCompression { get; set; }
    }
}
