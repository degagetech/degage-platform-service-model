using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    public class BasePacket
    {

        public String Authentication { get; set; }
        public String SessionId { get; set; }
    }
}
