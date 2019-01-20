using GBAHL.Utils;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace GBAHL
{
    /// <summary>
    /// Provides methods and properties for handling GBA ROM files.
    /// </summary>
    public sealed class RomFileInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RomFileInfo"/> class for the specified file.
        /// </summary>
        /// <param name="filename">The full name and path of the file.</param>
        /// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
        public RomFileInfo(string filename)
        {
            FileName = filename ?? throw new ArgumentNullException(nameof(filename));
            if (Exists)
            {
                try
                {
                    using (var fs = File.Open(filename, FileMode.Open, FileAccess.Read))
                    using (var br = new BinaryReader(fs))
                    {
                        br.BaseStream.Seek(0xA0L, SeekOrigin.Begin);
                        Title = StringHelper.ToString(br.ReadBytes(12), false);
                        Code  = StringHelper.ToString(br.ReadBytes(4), false);
                        Maker = StringHelper.ToString(br.ReadBytes(2), false);
                    }
                }
                catch { }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private FileStream Open(FileAccess access, FileShare share)
        {
            return File.Open(FileName, FileMode.Open, access, share);
        }

        /// <summary>
        /// Opens the file with read access.
        /// </summary>
        /// <returns></returns>
        public FileStream OpenRead()
        {
            return Open(FileAccess.Read, FileShare.ReadWrite);
        }

        /// <summary>
        /// Opens the file with write access.
        /// </summary>
        /// <returns></returns>
        public FileStream OpenWrite()
        {
            return Open(FileAccess.Write, FileShare.ReadWrite);
        }

        /// <summary>
        /// Opens the file with read/write access.
        /// </summary>
        /// <returns></returns>
        public FileStream OpenReadWrite()
        {
            return Open(FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        /// <summary>
        /// Gets the title of the game.
        /// </summary>
        public string Title { get; } = string.Empty;

        /// <summary>
        /// Gets the code of the game.
        /// </summary>
        public string Code { get; }  = string.Empty;

        /// <summary>
        /// Gets the maker of the game.
        /// </summary>
        public string Maker { get; } = string.Empty;

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Gets whether the file exists.
        /// </summary>
        public bool Exists => File.Exists(FileName);
    }
}
