using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Degage.ServiceModel.Rpc
{
    public class JsonTransferSerializer : ITransferSerializer
    {
        private static JsonSerializerSettings _Settings = null;

        public static NullValueHandling NullValueHandling { get; private set; } = NullValueHandling.Ignore;
        public static String DateFormatString { get; private set; } = "yyyy-MM-dd HH:mm:ss:fff";


        static JsonTransferSerializer()
        {
            _Settings = new JsonSerializerSettings();
            _Settings.NullValueHandling = JsonTransferSerializer.NullValueHandling;
            _Settings.DateFormatString = JsonTransferSerializer.DateFormatString;
        }

        public ITransportableObject SerializeInvokePacket(InvokePacket invokePacket)
        {
            JsonTransportable transportable = null;
            var jsonData = JsonConvert.SerializeObject(invokePacket, _Settings);
            transportable = new JsonTransportable(jsonData);
            return transportable;
        }

        public InvokePacket DeSerializeToInvokePacket(ITransportableObject tranObj)
        {
            InvokePacket packet = null;
            var jsonTranObj = tranObj as JsonTransportable;
            var jsonString = jsonTranObj.JsonString;
            if (jsonString == null) return packet;
            JObject jObject = JObject.Parse(jsonString);
            JsonSerializer serializer = JsonSerializer.Create(_Settings);
            var jtoken = jObject.GetValue(nameof(InvokePacket.ParameterValues));

            //若有传入参数
            if (jtoken != null)
            {
                jObject.Remove(nameof(InvokePacket.ParameterValues));
                packet = jObject.ToObject<InvokePacket>(serializer);

                Type[] parameterTypes = new Type[packet.ParameterTypes.Length];
                Object[] parameterValues = new Object[packet.ParameterTypes.Length];

                for (Int32 i = 0; i < packet.ParameterTypes.Length; ++i)
                {
                    parameterTypes[i] = Utilities.GetType(packet.ParameterTypes[i], true);
                }

                var jarrayObject = jtoken as JArray;
                if (packet.ParameterTypes.Length == 1)
                {
                    if (parameterTypes[0].IsArray)
                    {
                        parameterValues[0] = jarrayObject.ToObject(parameterTypes[0], serializer);
                    }
                    else
                    {
                        parameterValues[0] = jarrayObject[0].ToObject(parameterTypes[0], serializer);
                    }

                }
                else
                {
                    for (Int32 i = 0; i < parameterTypes.Length; ++i)
                    {
                        var jtokenObject = jarrayObject[i];
                        parameterValues[i] = jtokenObject.ToObject(parameterTypes[i], serializer);
                    }

                }
                packet.ParameterValues = parameterValues;
            }
            else
            {
                packet = jObject.ToObject<InvokePacket>(serializer);
            }

            return packet;
        }

        public ITransportableObject SerializeReturnPacket(ReturnPacket returnPacket)
        {
            JsonTransportable transportable = null;
            var jsonData = JsonConvert.SerializeObject(returnPacket, _Settings);
            transportable = new JsonTransportable(jsonData);
            return transportable;
        }

        public ReturnPacket DeSerializeToReturnPacket(ITransportableObject tranObj)
        {
            ReturnPacket packet = null;
            var jsonTranObj = tranObj as JsonTransportable;
            var jsonString = jsonTranObj.JsonString;
            if (jsonString == null) return packet;
            JsonSerializer serializer = JsonSerializer.Create(_Settings);
            JObject jObject = JObject.Parse(jsonString);

            var jtoken = jObject.GetValue(nameof(ReturnPacket.Content));
            //如果返回参数不为空
            if (jtoken != null)
            {
                jObject.Remove(nameof(ReturnPacket.Content));
                packet = jObject.ToObject<ReturnPacket>(serializer);

                Type contentType = Utilities.GetType(packet.ContentType, true);
                packet.Content = jtoken.ToObject(contentType, serializer);
            }
            else
            {
                packet = packet = jObject.ToObject<ReturnPacket>(serializer);
            }

            return packet;
        }

        public ITransportableObject CreateTransportable()
        {
            return new JsonTransportable();
        }

        public ITransportableObject CreateTransportable(Object orginal)
        {
            String jsonString = orginal as String;
            if (jsonString == null)
            {
                //参数错误
                throw new ArgumentException(nameof(orginal));
            }
            return new JsonTransportable(jsonString);
        }
    }
}
