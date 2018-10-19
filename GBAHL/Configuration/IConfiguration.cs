using System;
using System.IO;

namespace GBAHL.Configuration
{
    /// <summary>
    /// Provides a mechanism for loading and saving configuration files.
    /// </summary>
    public interface IConfiguration
    {
        T Load<T>(Stream stream);
        T LoadString<T>(string s);
        T LoadFile<T>(string filename);
        T LoadFile<T>(FileInfo file);
        void Save<T>(Stream stream, T config);
        string SaveString<T>(T config);
        void SaveFile<T>(string filename, T config);
        void SaveFile<T>(FileInfo file, T config);
    }
}
