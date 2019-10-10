using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
namespace Degage.ServiceModel.Rpc
{
    /// <summary>
    /// 代理类调用处理接口
    /// </summary>
    public interface IInvokeHandler : IDisposable
    {
        void Initialize();
        /// <summary>
        /// 处理对代理类方法的调用
        /// </summary>
        /// <param name="proxy">代理类的对象</param>
        /// <param name="interfaceType">接口的类型</param>
        /// <param name="invokeMethod">调用的接口的方法</param>
        /// <param name="parameterValues">方法参数值列表</param>
        /// <returns>方法的返回结果</returns>
        Object InovkeHandle(Object proxy, Type interfaceType, MethodInfo invokeMethod, Object[] parameterValues);
    }
}
