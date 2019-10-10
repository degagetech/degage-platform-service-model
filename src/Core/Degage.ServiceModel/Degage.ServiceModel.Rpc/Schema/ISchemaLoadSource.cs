using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    public interface ISchemaLoadSource
    {
        ServiceInterfaceSchema GetServiceInterfaceSchema(Type interfaceType);
        ServiceParameterSchema[] GetServiceInvokeParameterSchemas(Type interfaceType, MethodInfo operation);
        ServiceParameterSchema GetServiceReturnParameterSchema(Type interfaceType, MethodInfo operation);
        ServiceImplementSchema GetServiceImplementSchema(Type serviceImplementType);
        ServiceOperationSchema GetServiceOperationSchema(Type interfaceType, MethodInfo operation);
    }
}
