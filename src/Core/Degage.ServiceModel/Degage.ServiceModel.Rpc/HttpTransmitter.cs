using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Net.Cache;
using System.IO;

namespace Degage.ServiceModel.Rpc
{
    public class HttpTransmitter : ITransmitter
    {
        public CommunicationState State { get; private set; }
        public TransferSetting TransferSetting { get; private set; }

        public HttpTransmitter()
        {
            this.State = CommunicationState.Created;
        }


        public void Open(TransferSetting setting)
        {
            this.TransferSetting = setting;
            this.State = CommunicationState.Opened;
        }

        public Boolean Transfer(ITransportableObject tranObject, ITransportableObject retrunTranObject)
        {
            Boolean getted = false;

            Uri uri = new Uri(this.TransferSetting.Address);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.SendChunked = true;
            request.Timeout = (Int32)this.TransferSetting.Timeout.TotalMilliseconds;
            var stream = tranObject.GetStream();
            request.ContentLength = stream.Length;
            using (var requestStream = request.GetRequestStream())
            {
                stream.CopyTo(requestStream);
            }
            using (var response = request.GetResponse())
            {
                if (response.ContentLength > 0)
                {
                    using (var receiveStream = response.GetResponseStream())
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            receiveStream.CopyTo(memoryStream);
                            retrunTranObject.Load(memoryStream, 0, memoryStream.Length);
                        }
                        getted = true;
                    }
                }
            }

            return getted;
        }

        public void Dispose()
        {
            this.Close();
        }

        public void Close()
        {
            this.State = CommunicationState.Closed;
        }

        public object Transfer(ITransportableObject data)
        {
            throw new NotImplementedException();
        }
    }
}
