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
        T Load<T>(string s);
        void Save<T>(Stream stream, T config);
        void Save<T>(TextWriter textWriter, T config);
    }
}
