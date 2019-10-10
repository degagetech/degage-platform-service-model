using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    public interface ITransmitChannel
    {
        Boolean CanRead { get; }
        Boolean CanWrite { get; }

        Int32 Read(Byte[] buffer, Int32 offset, Int32 count);
        Int32 Write(Byte[] buffer, Int32 offset, Int32 count);

        Byte[] ReadAll();

        void Close();
    }
}
