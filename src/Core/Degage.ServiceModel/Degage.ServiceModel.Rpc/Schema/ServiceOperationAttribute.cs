using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Degage.ServiceModel.Rpc
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ServiceOperationAttribute : System.Attribute
    {
        public String Name { get; set; }

        public ServiceOperationAttribute() : base()
        {

        }
        public ServiceOperationAttribute(String name) : this()
        {
            this.Name = name;
        }


    }
}
