using System;
using System.IO;

namespace GBAHL.IO
{
    /// <summary>
    /// Helper methods for ROM files.
    /// </summary>
    public static class ROM
    {
        /// <summary>
        /// Returns the name, code, and maker for the specified ROM file.
        /// </summary>
        /// <param name="file">The file name and path of the ROM file.</param>
        /// <returns>
        /// A <see cref="ValueTuple{string, string, string}"/>
        /// containing the name, code, and maker strings.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
        /// <exception cref="FileNotFoundException">the file does not exist.</exception>
        public static (string Name, string Code, string Maker)
            GetHeader(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            return GetHeader(new FileInfo(filename));
        }

        /// <summary>
        /// Returns the name, code, and maker for the specified ROM file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        /// A <see cref="ValueTuple{string, string, string}"/>
        /// containing the name, code, and maker strings.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="file"/> is null.</exception>
        /// <exception cref="FileNotFoundException">the file does not exist.</exception>
        public static (string Name, string Code, string Maker)
            GetHeader(FileInfo file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            string name, code, maker;

            using (var reader = new BinaryReader(file.OpenRead()))
            {
                // NOTE: Assumes file is large enough for a ROM
                reader.BaseStream.Seek(0xA0L, SeekOrigin.Begin);
                name = reader.ReadString(12);
                code = reader.ReadString(4);
                maker = reader.ReadString(2);
            }

            return (name, code, maker);
        }

        /// <summary>
        /// Replaces the name, code, and/or maker for the specified ROM file.
        /// </summary>
        /// <param name="filename">The file name and path of the ROM file.</param>
        /// <param name="name">The new name; if null, nothing is written.</param>
        /// <param name="code">The new code; if null, nothing is written.</param>
        /// <param name="maker">The new maker; if null, nothing is written.</param>
        /// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
        /// <exception cref="FileNotFoundException">the file does not exist.</exception>
        public static void SetHeader(string filename, string name = null,
            string code = null, string maker = null)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            SetHeader(new FileInfo(filename), name, code, maker);
        }

        /// <summary>
        /// Replaces the name, code, and/or maker for the specified ROM file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="name">The new name; if null, nothing is written.</param>
        /// <param name="code">The new code; if null, nothing is written.</param>
        /// <param name="maker">The new maker; if null, nothing is written.</param>
        /// <exception cref="ArgumentNullException"><paramref name="file"/> is null.</exception>
        /// <exception cref="FileNotFoundException">the file does not exist.</exception>
        public static void SetHeader(FileInfo file, string name = null,
            string code = null, string maker = null)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            using (var writer = new BinaryWriter(file.OpenWrite()))
            {
                writer.Seek(0xA0, SeekOrigin.Begin);

                if (name != null)
                    writer.WriteString(name, 12);
                else
                    writer.Seek(12, SeekOrigin.Current);

                if (code != null)
                    writer.WriteString(code, 4);
                else
                    writer.Seek(4, SeekOrigin.Current);

                if (maker != null)
                    writer.WriteString(maker, 2);
            }
        }

        /// <summary>
        /// Returns whether the specified file is a valid ROM size.
        /// </summary>
        /// <param name="file">The file to check.</param>
        /// <returns></returns>
        public static bool IsValidSize(FileInfo file)
        {
            return file.Length % 0x1000000 == 0;
        }
    }
}
