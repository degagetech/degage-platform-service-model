using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Degage.ServiceModel.Rpc
{


    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class ServiceInterfaceAttribute : System.Attribute
    {
        public ServiceInterfaceAttribute() : base()
        {

        }
        public ServiceInterfaceAttribute(String name) : this()
        {
            this.Name = name;
        }
        public String Name { get; set; }

    }
}
