using Newtonsoft.Dson;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Server_API.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace Server_API.Formatters
{
    public class DsonFormatter : BufferedMediaTypeFormatter
    {
        public DsonFormatter()
        {
            // Add the supported media type
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/dson"));
        }

        public override bool CanReadType(Type type)
        {
            // Don't allow any dson receiving
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            // Encode anything as Dson
            return true;
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            using (var writer = new StreamWriter(writeStream))
            {
                writer.Write(DsonConvert.SerializeObject(value));
            }
        }
    }
}