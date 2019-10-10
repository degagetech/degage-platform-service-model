using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    /// <summary>
    /// 创建与管理服务模型所抛出的异常
    /// </summary>
    internal static class InternalExceptionManager
    {
        /// <summary>
        /// 使用指定的编码创建异常对象
        /// </summary>
        /// <param name="exceptionCode">异常编码</param>
        /// <returns></returns>
        internal static Exception NewException(this Int32 exceptionCode, Exception innerException = null)
        {
            Exception exception = null;
            var exceptionDesc = ServiceModelTextDescription.ResourceManager.GetString("Exception_Desc_" + exceptionCode.ToString("X"));
            if (exceptionDesc == null)
            {
                exceptionDesc = "Service Model happend exception!";
            }
            exception = new Exception(exceptionDesc, innerException);
            return exception;
        }
        internal static Exception NewException(this Int32 exceptionCode, String[] formatInfos, Exception innerException = null)
        {
            Exception exception = null;
            String exceptionDesc = null;
            var exceptionDescFormat = ServiceModelTextDescription.ResourceManager.GetString("Exception_Desc_" + exceptionCode.ToString("X"));
            if (exceptionDescFormat == null)
            {
                exceptionDesc = "Service Model happend exception!";
            }
            else
            {
                exceptionDesc = String.Format(exceptionDescFormat, formatInfos);
            }

            exception = new Exception(exceptionDesc, innerException);
            return exception;
        }
    }
}
