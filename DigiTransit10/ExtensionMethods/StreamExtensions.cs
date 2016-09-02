using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DigiTransit10.ExtensionMethods
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Serializes the given value to JSON, and then flushes it into this stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> that the serialized JSON data will be written to.</param>
        /// <param name="value">The value to serialize to JSON.</param>
        public static void SerializeJsonToStream(this Stream stream, object value)
        {
            JsonSerializer ser = new JsonSerializer();

            using (StreamWriter writer = new StreamWriter(stream))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {                
                ser.Serialize(jsonWriter, value);
                jsonWriter.Flush();
            }
        }

        public static object DeserializeJsonFromStream(this Stream stream)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize(jsonTextReader);
            }
        }

        public static T DeseriaizeJsonFromStream<T>(this Stream stream)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<T>(jsonTextReader);
            }
        }
    }
}
