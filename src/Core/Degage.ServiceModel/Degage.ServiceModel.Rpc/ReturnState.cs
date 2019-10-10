using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Biobank
{
    /// <summary>
    /// 常用服务调用返回结果的状态码
    /// </summary>
    public class ReturnState
    {
        /// <summary>
        /// 未知错误
        /// </summary>
        public const Int32 Unkown = -1;

        /// <summary>
        /// 表示服务返回正常
        /// </summary>
        public const Int32 Ok = 0;
        /// <summary>
        /// 服务执行时发生异常
        /// </summary>
        public const Int32 Expcetion = 0x10000;

        
        /// <summary>
        /// 会话信息无效
        /// </summary>
        public const Int32 InvalidSessionId = 0x10002;

        /// <summary>
        /// 服务调用传入的参数无效
        /// </summary>
        public const Int32 ArgumentInvaild = 0x10004;

        /// <summary>
        /// 连接失败
        /// </summary>
        public const Int32 ConnectFailed = 0x10005;

        /// <summary>
        /// 连接异常
        /// </summary>
        public const Int32 ConnectExpcetion = 0x10006;

        /// <summary>
        /// 无查询结果
        /// </summary>
        public const Int32 NoQueryResults = 0x10007;

        /// <summary>
        /// 数据操作失败
        /// </summary>
        public const Int32 DataOperationFailed = 0x10008;

        /// <summary>
        /// 系统配置错误
        /// </summary>
        public const Int32 ConfigError = 0x10009;

        /// <summary>
        /// 操作无效
        /// </summary>
        public const Int32 OperationInvalid = 0x10010;

        /// <summary>
        /// 数据异常
        /// </summary>
        public const Int32 DataExpcetion = 0x10011;

        /// <summary>
        /// 重复操作
        /// </summary>
        public const Int32 RepeatOperation = 0x10012;

        /// <summary>
        /// 外部接口错误
        /// </summary>
        public const Int32 ExternalInterfaceError = 0x10013;
    }
}
