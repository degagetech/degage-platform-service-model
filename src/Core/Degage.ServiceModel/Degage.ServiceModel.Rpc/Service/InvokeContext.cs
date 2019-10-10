using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Degage.ServiceModel.Rpc
{
    /// <summary>
    /// 调用上下文信息
    /// </summary>
    public class InvokeContext
    {

        /// <summary>
        /// 获取当前的调用上下文信息
        /// </summary>
        public static InvokeContext Current
        {
            get
            {
                return _Current;
            }
            set
            {
                _Current = value;
            }
        }


        [ThreadStatic]
        private static InvokeContext _Current;

        /// <summary>
        /// 当前调用的操作信息
        /// </summary>
        public InvokeOperationInfo OperationInfo { get; set; }


    }
}
