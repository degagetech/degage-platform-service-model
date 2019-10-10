using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
namespace Degage.ServiceModel.Rpc
{
    /// <summary>
    ///  从特性上加载服务模型结构信息的加载源
    /// </summary>
    public class AttributeServiceModelSchemaLoadSource : ISchemaLoadSource
    {
        public ServiceInterfaceSchema GetServiceInterfaceSchema(Type interfaceType)
        {
            ServiceInterfaceSchema schema = new ServiceInterfaceSchema();

            schema.Name = interfaceType.Name;
            schema.InterfaceType = interfaceType;

            Object[] attributes = interfaceType.GetCustomAttributes(typeof(ServiceInterfaceAttribute), true);
            if (attributes.Length > 0)
            {
                var attribute = attributes[0] as ServiceInterfaceAttribute;
                schema.Name = attribute.Name;
            }

            return schema;
        }

        public ServiceParameterSchema[] GetServiceInvokeParameterSchemas(Type interfaceType, MethodInfo operation)
        {
            ServiceParameterSchema[] schemas = null;
            var parameterInfos = operation.GetParameters();
            schemas = new ServiceParameterSchema[parameterInfos.Length];
            for (Int32 i = 0; i < parameterInfos.Length; ++i)
            {
                schemas[i] = new ServiceParameterSchema(parameterInfos[i]);
            }
            return schemas;
        }


        public ServiceParameterSchema GetServiceReturnParameterSchema(Type interfaceType, MethodInfo operation)
        {
            ServiceParameterSchema schema = null;
            var parameterInfo = operation.ReturnParameter;
            schema = new ServiceParameterSchema(parameterInfo);
            return schema;
        }
        public ServiceImplementSchema GetServiceImplementSchema(Type serviceImplementType)
        {
            ServiceImplementSchema schema = new ServiceImplementSchema();

            schema.ImplementType = serviceImplementType;
            schema.InstantiateMode = InstantiateMode.EachCall;

            Object[] attributes = serviceImplementType.GetCustomAttributes(typeof(ServiceImplementAttribute), true);
            if (attributes.Length > 0)
            {
                var attribute = attributes[0] as ServiceImplementAttribute;
                schema.InstantiateMode = attribute.InstantiateMode;
            }
            return schema;
        }
        public ServiceOperationSchema GetServiceOperationSchema(Type interfaceType, MethodInfo operation)
        {
            ServiceOperationSchema schema = new ServiceOperationSchema();

            schema.Name = operation.Name;
            schema.MethodInfo = operation;

            Object[] attributes = operation.GetCustomAttributes(typeof(ServiceOperationAttribute), true);
            if (attributes.Length > 0)
            {
                var attribute = attributes[0] as ServiceOperationAttribute;
                schema.Name = attribute.Name;
            }
            return schema;
        }
    }
}
