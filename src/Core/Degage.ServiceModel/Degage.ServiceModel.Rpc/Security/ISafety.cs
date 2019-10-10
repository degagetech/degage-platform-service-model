using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    public interface ISafety
    {
        String EncryptString(String orginal);
        String DecryptString(String orginal);
        Byte[] EncryptBuffer(Byte[] orginal);
        Byte[] DecryptBuffer(Byte[] orginal);
    }
}
