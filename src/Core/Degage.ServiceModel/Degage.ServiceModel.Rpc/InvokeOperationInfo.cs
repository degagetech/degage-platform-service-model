using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    /// <summary>
    /// 包含操作调用相关的信息
    /// </summary>
    public class InvokeOperationInfo
    {
        /// <summary>
        /// 调用操作所属接口的结构信息
        /// </summary>
        public ServiceInterfaceSchema InterfaceSchema { get; set; }

        /// <summary>
        /// 调用操作的结构信息
        /// </summary>
        public ServiceOperationSchema OperationSchema { get; set; }

        /// <summary>
        /// 调用操作传入参数的信息
        /// </summary>
        public ServiceParameterSchema[] InvokeParameterSchemas { get; set; }

        /// <summary>
        /// 调用操作返回参数的信息
        /// </summary>
        public ServiceParameterSchema ReturnParameterSchema { get; set; }

        /// <summary>
        /// 使用指定服务接口的类型以及接口中的方法信息，创建调用操作信息
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <param name="operationInfo">操作方法信息</param>
        /// <returns></returns>
        public static InvokeOperationInfo Create(Type interfaceType, MethodInfo operationInfo)
        {
            InvokeOperationInfo info = new InvokeOperationInfo();
            Fill(info, interfaceType, operationInfo);
            return info;
        }
        internal static void Fill(InvokeOperationInfo info, Type interfaceType, MethodInfo operationInfo)
        {
            var interfaceSchema = ServiceModelSchema.GetServiceInterfaceSchema(interfaceType);
            var operationSchema = ServiceModelSchema.GetServiceOperationSchema(interfaceType, operationInfo);
            var invokeParameterSchemas = ServiceModelSchema.GetServiceInvokeParameterSchemas(interfaceType, operationInfo);
            var returnParameterSchema = ServiceModelSchema.GetServiceReturnParameterSchema(interfaceType, operationInfo);

            info.InterfaceSchema = interfaceSchema;
            info.OperationSchema = operationSchema;
            info.InvokeParameterSchemas = invokeParameterSchemas;
            info.ReturnParameterSchema = returnParameterSchema;
        }
    }
}
