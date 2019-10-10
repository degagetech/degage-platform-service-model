using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    public class ServiceModelException : Exception
    {
        public Int32 ErrorCode { get; set; }
        public ServiceModelException(String message, Exception innerExceotion) : base(message, innerExceotion)
        {

        }

        public ServiceModelException(Int32 errorCode, String message, Exception innerExceotion) : base(message, innerExceotion)
        {
            this.ErrorCode = errorCode;
        }
    }
}
