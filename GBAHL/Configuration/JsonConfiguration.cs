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

        public T LoadString<T>(string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            using (var sr = new StringReader(s))
            using (var jr = new JsonTextReader(sr))
            {
                return serializer.Deserialize<T>(jr);
            }
        }

        public T LoadFile<T>(string filename)
        {
            return LoadFile<T>(new FileInfo(filename));
        }

        public T LoadFile<T>(FileInfo file)
        {
            using (var stream = file.OpenRead())
            {
                return Load<T>(stream);
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

        public string SaveString<T>(T config)
        {
            using (var sw = new StringWriter())
            using (var jw = new JsonTextWriter(sw))
            {
                serializer.Serialize(jw, config);
                return sw.ToString();
            }
        }

        public void SaveFile<T>(string filename, T config)
        {
            SaveFile<T>(new FileInfo(filename), config);
        }

        public void SaveFile<T>(FileInfo file, T config)
        {
            using (var stream = file.Create())
            {
                Save<T>(stream, config);
            }
        }
    }
}
