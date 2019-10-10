using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    public enum CommunicationState
    {
        Created = 0,
        Opening = 1,
        Opened = 2,
        Closing = 3,
        Closed = 4,
        Faulted = 5
    }
}
