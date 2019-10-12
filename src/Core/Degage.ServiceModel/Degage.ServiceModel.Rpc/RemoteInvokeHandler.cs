#define SerializePerformance_
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Diagnostics;
namespace Degage.ServiceModel.Rpc
{
    public class RemoteInvokeHandler : IInvokeHandler
    {
        public ProxySetting ProxySetting { get; internal set; }
        public ITransferSerializer Serializer { get; internal set; }
        public ITransmitter Transmitter { get; internal set; }

        public RemoteInvokeHandler() { }

        public RemoteInvokeHandler(ProxySetting setting, ITransferSerializer serializer, ITransmitter transmitter) : this()
        {
            this.ProxySetting = setting;
            this.Serializer = serializer;
            this.Transmitter = transmitter;
        }

        public Object InovkeHandle(Object proxy, Type interfaceType, MethodInfo invokeMethod, Object[] parameterValues)
        {
            Object result = null;
            try
            {
                InvokeOperationInfo invokeInfo = InvokeOperationInfo.Create(interfaceType, invokeMethod);

                InvokePacket invokePacket = new InvokePacket();
                invokePacket.InterfaceName = invokeInfo.InterfaceSchema.Name;
                invokePacket.OperationName = invokeInfo.OperationSchema.Name;

                invokePacket.ParameterValues = parameterValues;
                invokePacket.ParameterTypes = invokeInfo.InvokeParameterSchemas.Select(t => t.ParameterType.FullName).ToArray();

                invokePacket.InvokeTime = DateTime.Now;
#if SerializePerformance
                Debug.WriteLine("SerializeInvokePacket Start Time: " + DateTime.Now.ToString("ss:fff"));
                var invokeTransportableObject = this.Serializer.SerializeInvokePacket(invokePacket);
                Debug.WriteLine("SerializeInvokePacket End Time: " + DateTime.Now.ToString("ss:fff"));
#else
                var invokeTransportableObject = this.Serializer.SerializeInvokePacket(invokePacket);
#endif
                var returnTransportableObject = this.Serializer.CreateTransportable();

                if (this.Transmitter.Transfer(invokeTransportableObject, returnTransportableObject))
                {
                    //调用用户自定义过程

#if SerializePerformance
                    Debug.WriteLine("DeSerialize Return Packet Start Time: " + DateTime.Now.ToString("ss:fff"));
                    var returnPacket = this.Serializer.DeSerializeToReturnPacket(returnTransportableObject);
                    result = returnPacket.Content;
                    Debug.WriteLine("DeSerialize Return Packet End Time: " + DateTime.Now.ToString("ss:fff"));
#else
                    var returnPacket = this.Serializer.DeSerializeToReturnPacket(returnTransportableObject);
                    result = returnPacket.Content;
#endif



                }

            }
            finally
            {

            }
            return result;
        }

        public void Dispose()
        {
            try
            {
                this.Transmitter.Dispose();
            }
            catch
            {
            }
            finally
            {
                this.Transmitter = null;
            }
        }

        public void Initialize()
        {
            this.Transmitter.Open(this.ProxySetting.TransferSetting);
        }
    }
}
