using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    public class ServiceParameterAttribute : System.Attribute
    {
        public String Name { get; set; }
    }
}
