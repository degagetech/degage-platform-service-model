using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    /// <summary>
    /// 用于传输远程调用数据
    /// </summary>
    public interface ITransmitter : IDisposable
    {
        CommunicationState State { get; }
        TransferSetting TransferSetting { get; }
        /// <summary>
        /// 使用指定的传输设置打开传输器
        /// </summary>
        /// <param name="setting"></param>
        void Open(TransferSetting setting);
        /// <summary>
        /// 使用指定的传输设置传输数据
        /// </summary>
        /// <param name="tranObject">包含向终点传输信息的可传输对象</param>
        /// <param name="retrunTranObject">包含终点返回信息的可传输对象</param>
        /// <returns>终点是否返回信息</returns>
        Boolean Transfer(ITransportableObject tranObject, ITransportableObject retrunTranObject);

        /// <summary>
        /// 关闭传输器并释放相关资源
        /// </summary>
        void Close();
    }
}
