using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    /// <summary>
    /// 包含复杂对象便于传输的形态信息
    /// </summary>
    public interface ITransportableObject
    {
        /// <summary>
        ///加载指定数据流中的信息，此函数不负责传入流的资源释放
        /// </summary>
        /// <param name="stream"></param>
        void Load(Stream stream, Int64 offset, Int64 count);
        /// <summary>
        /// 可传输对象包含的原始信息
        /// </summary>
        Object Orignal { get; }
        /// <summary>
        /// 获取包含可传输对象信息的新的流对象
        /// </summary>
        Stream GetStream();
    }
}
