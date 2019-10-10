using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    public abstract class BaseServiceHost : IDisposable
    {
        public abstract void Open(TransferSetting setting);
        public abstract void Close();

        public TransferSetting TransferSetting { get; protected set; }
        /// <summary>
        /// 注册指定的服务类，一个接口类型只能被注册一次
        /// </summary>
        /// <param name="interfaceType">服务接口的类型</param>
        /// <param name="serviceType">服务接口实现类的类型</param>
        public abstract void Register(Type serviceInterfaceType, Type serviceImplementType);


    

        //protected Object InvokeHandle(Type serviceType, Type interfaceType, MethodInfo info, Object[] invokeParameters)
        //{

        //}



        public void Dispose()
        {
            this.Close();
        }
    }
}
