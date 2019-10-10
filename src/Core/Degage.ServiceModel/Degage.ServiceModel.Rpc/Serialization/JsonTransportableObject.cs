using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Degage.ServiceModel.Rpc
{
    public class JsonTransportable : ITransportableObject
    {
        internal Encoding Encoding { get; set; }

        internal String JsonString { get; private set; }



        public Object Orignal
        {
            get
            {
                return this.JsonString;
            }
        }

        internal JsonTransportable()
        {
            this.Encoding = UTF8Encoding.UTF8;
        }
        public JsonTransportable(String jsonString) : this()
        {
            this.JsonString = jsonString;
        }

        public JsonTransportable(Stream stream, Int64 offset, Int64 count) : this()
        {
            this.Load(stream, offset, count);
        }
        public Stream GetStream()
        {
            Byte[] buffer = this.Encoding.GetBytes(this.JsonString);
            Stream stream = new MemoryStream(buffer);
            return stream;
        }

        public void Load(Stream stream, Int64 offset, Int64 count)
        {
            stream.Position = offset;
            Byte[] buffer = new Byte[count];
            stream.Read(buffer, 0, (Int32)count);
            this.JsonString = this.Encoding.GetString(buffer);
        }
    }
}
