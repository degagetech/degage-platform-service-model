using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Reflection;
namespace Degage.ServiceModel.Rpc
{

    /// <summary>
    /// 管理服务模型的结构信息
    /// </summary>
    public static class ServiceModelSchema
    {
        /// <summary>
        /// 
        /// </summary>
        public static ISchemaLoadSource SchemaLoadSource { get; set; }

        private static Object _SyncObject = new Object();
        /// <summary>
        /// Key is interfaceType，Value is <see cref="ServiceInterfaceSchema"/> instance
        /// </summary>
        private static ConcurrentDictionary<Int32, ServiceInterfaceSchema> _ServiceInterfaceSchemaTable;
        private static Object _ServiceInterfaceSchemaTableLock = new Object();

        /// <summary>
        /// Key  is methodInfo object hashcode
        /// </summary>
        private static ConcurrentDictionary<Int32, ServiceOperationSchema> _ServiceOperationSchemaTable;
        private static Object _ServiceOperationSchemaTableLock = new Object();
        /// <summary>
        /// Key  is methodInfo object hashcode
        /// </summary>
        private static ConcurrentDictionary<Int32, ServiceParameterSchema> _ServiceReturnParameterSchemaTable;
        private static Object _ServiceOperationReturnParameterLock = new Object();
        /// <summary>
        ///  Key  is methodInfo object hashcode
        /// </summary>
        private static ConcurrentDictionary<Int32, ServiceParameterSchema[]> _ServiceInvokeParameterSchemaTable;
        private static Object _ServiceOperationInvokeParameterLock = new Object();

        /// <summary>
        ///  Key  is service implement type hashcode
        /// </summary>
        private static ConcurrentDictionary<Int32, ServiceImplementSchema> _ServiceImplementSchemaTable;
        private static Object _ServiceImplementTableLock = new Object();



        static ServiceModelSchema()
        {
            _ServiceInterfaceSchemaTable = new ConcurrentDictionary<Int32, ServiceInterfaceSchema>();
            _ServiceOperationSchemaTable = new ConcurrentDictionary<Int32, ServiceOperationSchema>();
            _ServiceReturnParameterSchemaTable = new ConcurrentDictionary<Int32, ServiceParameterSchema>();
            _ServiceInvokeParameterSchemaTable = new ConcurrentDictionary<Int32, ServiceParameterSchema[]>();
            _ServiceImplementSchemaTable = new ConcurrentDictionary<Int32, ServiceImplementSchema>();

            SchemaLoadSource = new AttributeServiceModelSchemaLoadSource();
        }

        public static ServiceImplementSchema GetServiceImplementSchema(Type serviceImplementType)
        {
            ServiceImplementSchema schema = null;
            Int32 hashcode = serviceImplementType.GetHashCode();
            if (!_ServiceImplementSchemaTable.TryGetValue(hashcode, out schema))
            {
                lock (_ServiceImplementTableLock)
                {
                    if (!_ServiceImplementSchemaTable.TryGetValue(hashcode, out schema))
                    {
                        schema = ServiceModelSchema.SchemaLoadSource.GetServiceImplementSchema(serviceImplementType);
                        if (schema != null)
                        {
                            _ServiceImplementSchemaTable.TryAdd(hashcode, schema);
                        }
                        else
                        {
                            throw ExceptionCode.UnloadServiceSchema.NewException();
                        }
                    }
                }
            }
            return schema;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ServiceModelException">Thrown when unload schema info </exception>
        /// <param name="interfaceType"></param>
        /// <param name="info">interface's method info</param>
        /// <returns></returns>
        public static ServiceParameterSchema GetServiceReturnParameterSchema(Type interfaceType, MethodInfo info)
        {
            ServiceParameterSchema schema = null;
            Int32 hashcode = info.GetHashCode();
            if (!_ServiceReturnParameterSchemaTable.TryGetValue(hashcode, out schema))
            {
                lock (_ServiceOperationReturnParameterLock)
                {
                    if (!_ServiceReturnParameterSchemaTable.TryGetValue(hashcode, out schema))
                    {
                        schema = ServiceModelSchema.SchemaLoadSource.GetServiceReturnParameterSchema(interfaceType, info);
                        if (schema != null)
                        {
                            _ServiceReturnParameterSchemaTable.TryAdd(hashcode, schema);
                        }
                        else
                        {
                            throw ExceptionCode.UnloadServiceSchema.NewException();
                        }
                    }
                }
            }
            return schema;
        }


        public static ServiceParameterSchema[] GetServiceInvokeParameterSchemas(Type interfaceType, MethodInfo info)
        {
            ServiceParameterSchema[] schemas = null;
            Int32 hashcode = info.GetHashCode();
            if (!_ServiceInvokeParameterSchemaTable.TryGetValue(hashcode, out schemas))
            {
                lock (_ServiceOperationInvokeParameterLock)
                {
                    if (!_ServiceInvokeParameterSchemaTable.TryGetValue(hashcode, out schemas))
                    {
                        schemas = ServiceModelSchema.SchemaLoadSource.GetServiceInvokeParameterSchemas(interfaceType, info);
                        if (schemas != null)
                        {
                            _ServiceInvokeParameterSchemaTable.TryAdd(hashcode, schemas);
                        }
                        else
                        {
                            throw ExceptionCode.UnloadServiceSchema.NewException();
                        }
                    }
                }
            }
            return schemas;
        }

        public static ServiceInterfaceSchema GetServiceInterfaceSchema(Type interfaceType,Boolean thowError=false)
        {
            ServiceInterfaceSchema schema = null;
            Int32 hashcode = interfaceType.GetHashCode();
            if (!_ServiceInterfaceSchemaTable.TryGetValue(hashcode, out schema))
            {
                lock (_ServiceInterfaceSchemaTableLock)
                {
                    if (!_ServiceInterfaceSchemaTable.TryGetValue(hashcode, out schema))
                    {
                        schema = ServiceModelSchema.SchemaLoadSource.GetServiceInterfaceSchema(interfaceType);
                        if (schema != null)
                        {
                            _ServiceInterfaceSchemaTable.TryAdd(hashcode, schema);
                        }
                        else if(thowError)
                        {
                            throw ExceptionCode.UnidentifiedServiceInterfaceOnInterfaceType.NewException();
                        }
                    }
                }
            }
            return schema;
        }

        public static ServiceOperationSchema GetServiceOperationSchema(Type interfaceType, MethodInfo methodInfo, Boolean thowError = false)
        {
            ServiceOperationSchema schema = null;
            Int32 hashcode = methodInfo.GetHashCode();
            if (!_ServiceOperationSchemaTable.TryGetValue(hashcode, out schema))
            {
                lock (_ServiceOperationSchemaTableLock)
                {
                    if (!_ServiceOperationSchemaTable.TryGetValue(hashcode, out schema))
                    {
                        schema = ServiceModelSchema.SchemaLoadSource.GetServiceOperationSchema(interfaceType, methodInfo);
                        if (schema != null)
                        {
                            _ServiceOperationSchemaTable.TryAdd(hashcode, schema);
                        }
                        else if(thowError)
                        {
                            throw ExceptionCode.UnidentifiedServiceOperationOnMethod.NewException();
                        }

                    }
                }
            }
            return schema;
        }



    }
}
