using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Reflection;
using System.Linq.Expressions;
using System.IO;
namespace Degage.ServiceModel.Rpc
{
    public class HttpServiceHost
    {
        public TransferSetting TransferSetting { get; protected set; }

        public CommunicationState State { get; private set; }
        public Action<HostExceptionEventArgs> ExceptionHanppened;
        /// <summary>
        ///  Key is operation path
        /// </summary>
        private Dictionary<String, InvokeImplementOperationInfo> _pathOperationTable;
        /// <summary>
        /// Key is service interface type instance
        /// </summary>
        private HashSet<Type> _registerTable;
        /// <summary>
        /// Key is implement type,Value is service instace
        /// </summary>
        private Dictionary<Type, Object> _serviceInstanceTable;
        private Dictionary<Type, Func<Object>> _serviceInstanceCreateFactoryTable;

        private HttpListener _httpListener;
        private Thread _listenThread;
        private Boolean _closeFlag;
        private ITransferSerializer _transferSerializer;

        public HttpServiceHost()
        {
            this._httpListener = new HttpListener();
            this._listenThread = new Thread(HttpListen);
            this._closeFlag = false;

            this._pathOperationTable = new Dictionary<String, InvokeImplementOperationInfo>();
            this._registerTable = new HashSet<Type>();
            this._serviceInstanceTable = new Dictionary<Type, Object>();
            this._serviceInstanceCreateFactoryTable = new Dictionary<Type, Func<Object>>();
            this._transferSerializer = new JsonTransferSerializer();
        }
        protected InvokeImplementOperationInfo GetOperationInfo(String operationPath)
        {
            InvokeImplementOperationInfo info = null;
            if (operationPath != null && this._pathOperationTable.ContainsKey(operationPath))
            {
                info = this._pathOperationTable[operationPath];
            }
            return info;
        }





        /// <summary>
        /// 注册指定类型的服务接口，以及其对应的服务实现的类型，重复注册时将被忽略
        /// </summary>
        /// <param name="interfaceType">服务接口的类型</param>
        /// <param name="serviceType">服务实现的类型</param>
        public void Register(Type interfaceType, Type serviceType)
        {
            if (this.State != CommunicationState.Created) throw new Exception();

            if (this._registerTable.Contains(interfaceType)) return;


            if (!interfaceType.IsAssignableFrom(interfaceType))
            {
                throw ExceptionCode.NoMatchServiceInterfaceTypeWithImplementType.NewException();
            }

            var interfaceSchema = ServiceModelSchema.GetServiceInterfaceSchema(interfaceType);
            var methodInfos = interfaceType.GetMethods();

            Boolean loadedServiceInstace = false;
            foreach (var methodInfo in methodInfos)
            {
                InvokeImplementOperationInfo operationInfo = InvokeImplementOperationInfo.Create(interfaceType, methodInfo, serviceType);
                var path = Utilities.GetOperationPath(operationInfo.InterfaceSchema, operationInfo.OperationSchema);

                if (this._pathOperationTable.ContainsKey(path))
                {
                    var orginalOperationInfo = this._pathOperationTable[path];

                    throw ExceptionCode.ServiceOperationPathEqual.NewException(new String[] {
                                operationInfo.InterfaceSchema.InterfaceType.FullName,
                                operationInfo.OperationSchema.MethodInfo.ToString(),
                                orginalOperationInfo.InterfaceSchema.InterfaceType.FullName,
                                orginalOperationInfo.OperationSchema.MethodInfo.ToString()});
                }

                this._pathOperationTable.Add(path, operationInfo);

                if (loadedServiceInstace) continue;
                switch (operationInfo.ImplementSchema.InstantiateMode)
                {
                    case InstantiateMode.Singleton:
                        {
                            var instance = Activator.CreateInstance(operationInfo.ImplementSchema.ImplementType);
                            this._serviceInstanceTable.Add(operationInfo.ImplementSchema.ImplementType, instance);
                        }
                        break;
                    case InstantiateMode.EachCall:
                        {
                            Func<Object> serviceInstaceFactory = null;

                            Expression newExp = Expression.New(operationInfo.ImplementSchema.ImplementType);
                            Expression castExp = Expression.Convert(newExp, typeof(Object));
                            serviceInstaceFactory = Expression.Lambda<Func<Object>>(castExp).Compile();

                            this._serviceInstanceCreateFactoryTable.Add(
                                operationInfo.ImplementSchema.ImplementType,
                                serviceInstaceFactory
                                );
                        }
                        break;
                }
                loadedServiceInstace = true;
            }

        }
        public void Open(TransferSetting setting)
        {
            this.TransferSetting = setting;
            this._httpListener.Prefixes.Add(setting.Address);
            this._httpListener.Start();
            this._listenThread.Start();
        }

        protected void HttpListen(Object state)
        {
            while (!_closeFlag)
            {
                HttpListenerContext context = null;
                try
                {
                    context = this._httpListener.GetContext();

                    ThreadPool.QueueUserWorkItem(InvokeRoute, context);
                }
                catch
                {
                }
            }
        }

        // Interface【 Object InvokeRoute(Object data); //将调用路由到指定的服务实例上】
        private void InvokeRoute(Object state)
        {
            try
            {

                HttpListenerContext context = state as HttpListenerContext;

                //获取调用信息
                var invokeTranObject = this._transferSerializer.CreateTransportable();
                MemoryStream memoryStream = new MemoryStream();
                context.Request.InputStream.CopyTo(memoryStream);
                invokeTranObject.Load(memoryStream, 0, memoryStream.Length);
                memoryStream.Dispose();
                var invokePacket = this._transferSerializer.DeSerializeToInvokePacket(invokeTranObject);
                if (invokePacket == null)
                {
                    goto Close;
                }
                //获取服务操作的信息
                String operationPath = Utilities.GetOperationPath(invokePacket.InterfaceName, invokePacket.OperationName);
                var operationInfo = this.GetOperationInfo(operationPath);
                if (operationInfo == null)
                {
                    goto Close;
                }
                Object serviceInstace = null;
                switch (operationInfo.ImplementSchema.InstantiateMode)
                {
                    case InstantiateMode.EachCall:
                        {
                            Func<Object> serviceInstaceFactory = this._serviceInstanceCreateFactoryTable[operationInfo.ImplementSchema.ImplementType];
                            serviceInstace = serviceInstaceFactory.Invoke();
                        }
                        break;
                    case InstantiateMode.Singleton:
                        {
                            serviceInstace = this._serviceInstanceTable[operationInfo.ImplementSchema.ImplementType];
                        }
                        break;
                }
                var returnPacket = this.InvokeHandler(serviceInstace, invokePacket, operationInfo);

                var returnTranObject = this._transferSerializer.SerializeReturnPacket(returnPacket);
                var returnStream = returnTranObject.GetStream();

                context.Response.StatusCode = 200;
                context.Response.ContentLength64 = returnStream.Length;

                try
                {
                    returnStream.CopyTo(context.Response.OutputStream);
                }
                finally
                {
                    returnStream.Close();
                }
            Close:
                context.Response.Close();
            }
            catch (Exception exc)
            {
                HostExceptionEventArgs args = new HostExceptionEventArgs();
                args.Exception = exc;
                args.Handled = false;
                this.ExceptionHanppened?.Invoke(args);
                if (!args.Handled) throw exc;
            }
            //returnStream.CopyTo(context.Response.OutputStream);
            //returnStream.Close();
        }

        //interface  //处理调用
        protected virtual ReturnPacketInternal InvokeHandler(Object instance, InvokePacket packet, InvokeImplementOperationInfo operationInfo)
        {
            ReturnPacketInternal returnPacket = new ReturnPacketInternal();
            Object returnObject = null;
            try
            {
                returnPacket.HandleTime = DateTime.Now;
                returnObject = operationInfo.OperationSchema.MethodInfo.Invoke(instance, packet.ParameterValues);

                returnPacket.Success = true;
                returnPacket.Content = returnObject;
                returnPacket.ContentType = operationInfo.ReturnParameterSchema.ParameterType.FullName;
            }
            catch (Exception exc)
            {
                returnPacket.Success = false;
                returnPacket.Message = exc.Message;
                if (this.ExceptionHanppened != null)
                {
                    HostExceptionEventArgs eventArgs = new HostExceptionEventArgs();
                    eventArgs.Exception = exc;
                    this.ExceptionHanppened.Invoke(eventArgs);
                    if (!eventArgs.Handled)
                    {
                        throw exc;
                    }
                }
                else
                {
                    throw exc;
                }
            }
            returnPacket.Elapsed = (Int32)(DateTime.Now - returnPacket.HandleTime).TotalMilliseconds;
            return returnPacket;
        }
        public void Close()
        {
            this._httpListener.Close();

            try
            {
                if (this._listenThread.IsAlive)
                {
                    this._listenThread.Abort();
                }
            }
            catch
            {
            }

        }
    }
    public class HostExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception { get; internal set; }
        /// <summary>
        /// 是否已处理此异常
        /// </summary>
        public Boolean Handled { get; set; }
    }
}
