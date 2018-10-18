using Newtonsoft.Json;
using System;
using System.IO;

namespace GBAHL.Configuration
{
    public class JsonConfiguration : IConfiguration
    {
        public static JsonConfiguration Default => new JsonConfiguration();

        private JsonSerializer serializer;

        public JsonConfiguration(JsonSerializerSettings settings = null)
        {
            serializer = JsonSerializer.CreateDefault(settings);
        }

        public T Load<T>(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            using (var sr = new StreamReader(stream))
            using (var jr = new JsonTextReader(sr))
            {
                return serializer.Deserialize<T>(jr);
            }
        }

        public T Load<T>(string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            using (var sr = new StringReader(s))
            using (var jr = new JsonTextReader(sr))
            {
                return serializer.Deserialize<T>(jr);
            }
        }

        public void Save<T>(Stream stream, T config)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            using (var sw = new StreamWriter(stream))
            using (var jw = new JsonTextWriter(sw))
            {
                serializer.Serialize(jw, config);
            }
        }

        public void Save<T>(TextWriter textWriter, T config)
        {
            if (textWriter == null)
                throw new ArgumentNullException(nameof(textWriter));

            using (var jw = new JsonTextWriter(textWriter))
            {
                serializer.Serialize(jw, config);
            }
        }
    }
}
