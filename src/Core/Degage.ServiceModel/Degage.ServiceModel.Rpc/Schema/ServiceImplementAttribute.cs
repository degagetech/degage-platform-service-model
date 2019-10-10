using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    public class ServiceImplementAttribute : Attribute
    {
   
        public InstantiateMode InstantiateMode { get; set; }
    }
}
