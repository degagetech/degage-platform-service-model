using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    /// <summary>
    ///将复杂数据结构对象转换为可传输的数据对象，或者将可传输的数据对象转换为复杂结构数据对象
    /// </summary>
    public interface ITransferSerializer
    {
        /// <summary>
        /// 创建一个尚未包含任何信息的可传输对象
        /// </summary>
        /// <returns></returns>
        ITransportableObject CreateTransportable();

        /// <summary>
        /// 使用指定的原始信息创建一个可传输对象
        /// </summary>
        /// <param name="orignal"></param>
        /// <returns></returns>
        ITransportableObject CreateTransportable(Object orignal);

        /// <summary>
        /// 将指定的 <see cref="InvokePacket"/> 对象序列化为可传输对象
        /// </summary>
        /// <param name="invokeInfo"></param>
        /// <returns></returns>
        ITransportableObject SerializeInvokePacket(InvokePacket invokePacket);

        /// <summary>
        /// 将指定的可传输对象反序列化为 <see cref="InvokePacket"/> 对象
        /// </summary>
        /// <param name="invokeInfo"></param>
        /// <param name="tranObj"></param>
        /// <returns></returns>
        InvokePacket DeSerializeToInvokePacket(ITransportableObject tranObj);

        /// <summary>
        /// 将指定的 <see cref="ReturnPacketInternal "/> 对象序列化为可传输对象
        /// </summary>
        ITransportableObject SerializeReturnPacket(ReturnPacketInternal  returnPacket);

        /// <summary>
        /// 将指定的可传输对象反序列化为 <see cref="ReturnPacketInternal "/> 对象
        /// </summary>
        ReturnPacketInternal  DeSerializeToReturnPacket(ITransportableObject tranObj);

    }
}
