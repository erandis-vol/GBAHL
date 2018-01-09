using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using GBAHL.Text;
using GBAHL.Drawing;

namespace GBAHL.IO
{
    /// <summary>
    /// Reads and writes primitive data types to/from a ROM.
    /// </summary>
    public class ROM_old : BinaryStream_old
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ROM_old"/> class based on the specified file,
        /// with the default access and sharing options.
        /// </summary>
        /// <param name="filePath">The file.</param>
        /// <exception cref="FileNotFoundException">unable to open specified file.</exception>
        /// <exception cref="ArgumentException">file is larger than 0x1FFFFFF bytes.</exception>
        public ROM_old(string filePath) : this(filePath, FileAccess.ReadWrite, FileShare.ReadWrite)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ROM_old"/> class based on the specified file,
        /// with the specified read/write access and the the specified sharing option.
        /// </summary>
        /// <param name="filePath">The file.</param>
        /// <param name="access">A <see cref="FileAccess"/> value that specifies the actions that can be performed on the ROM.</param>
        /// <param name="share">A <see cref="FileShare"/> value specifying the type of access other threads have to the ROM.</param>
        /// <exception cref="FileNotFoundException">unable to open specified file.</exception>
        /// <exception cref="ArgumentException">file is larger than 0x1FFFFFF bytes.</exception>
        public ROM_old(string filePath, FileAccess access, FileShare share) : base(filePath, access, share)
        {
#if ENFORCE_ROM_SIZE
            // TODO: Factor of two or something
            if (Length % 0x1000000 != 0)
            {
                Dispose();
                throw new Exception("File is not the correct size for a ROM!");
            }
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ROM_old"/> class based on the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <exception cref="ArgumentException">stream is longer than 0x1FFFFFF bytes.</exception>
        public ROM_old(Stream stream) : base(stream)
        {
#if ENFORCE_ROM_SIZE
            if (Length % 0x1000000 != 0)
            {
                throw new Exception("Stream is not the correct size for a ROM!");
            }
#endif
        }

        #region Read

        /// <summary>
        /// Reads and validates a 4-byte ROM pointer from the stream and advances the position by four bytes.
        /// </summary>
        /// <returns>The offset pointed to if valid; -1 otherwise.</returns>
        public int ReadPointer()
        {
            // read value
            var ptr = ReadInt32();

            // return on blank pointer
            if (ptr == 0)
                return 0;

            // a pointer must be between 0x0 and 0x1FFFFFF to be valid on the GBA
            // ROM pointer format is OFFSET | 0x8000000, so 0x8000000 <= POINTER <= 0x9FFFFFF
            if (ptr < 0x8000000 || ptr > 0x9FFFFFF)
                return -1;

            // easy way to extract
            return ptr & 0x1FFFFFF;
        }

        /// <summary>
        /// Reads and validates a 4-byte ROM pointer from the stream. If valid, sets the position to that offset.
        /// </summary>
        /// <returns>The offset pointed to if valid; -1 otherwise.</returns>
        public int ReadPointerAndSeek()
        {
            int ptr = ReadPointer();
            if (ptr >= 0) {
                Seek(ptr, SeekOrigin.Begin);
                return ptr;
            }
            return -1;
        }

        /// <summary>
        /// Reads an FF-terminated string using the given <see cref="Encoding"/> and advances the position.
        /// </summary>
        /// <param name="encoding">The encoding of the string.</param>
        /// <returns></returns>
        public string ReadText(Table.Encoding encoding)
        {
            // read string until FF
            List<byte> buffer = new List<byte>();
            byte temp = 0;
            do
            {
                buffer.Add((temp = ReadByte()));
            } while (temp != 0xFF);

            // convert to string
            return Table.GetString(buffer.ToArray(), encoding);
        }

        /// <summary>
        /// Reads a string of the given length using the given <see cref="Table.Encoding"/> and advances the position by that many bytes.
        /// </summary>
        /// <param name="length">The length of the string.</param>
        /// <param name="encoding">The encoding of the string.</param>
        /// <returns></returns>
        public string ReadText(int length, Table.Encoding encoding)
        {
            return Table.GetString(ReadBytes(length), encoding);
        }

        /// <summary>
        /// Reads a table of strings of the given length using the given <see cref="Table.Encoding"/> and advances the position by that many bytes.
        /// </summary>
        /// <param name="stringLength">The length of the string.</param>
        /// <param name="tableSize">The length of the table.</param>
        /// <param name="encoding">The encoding of the string.</param>
        /// <returns></returns>
        public string[] ReadTextTable(int stringLength, int tableSize, Table.Encoding encoding)
        {
            var table = new string[tableSize];
            for (int i = 0; i < tableSize; i++)
            {
                table[i] = ReadText(stringLength, encoding);
            }
            return table;
        }

        /// <summary>
        /// Reads a 2-byte BGR555 color from the stream and advances the position by two bytes.
        /// </summary>
        /// <returns>A <see cref="Color"/> read from the stream.</returns>
        public Color ReadColor()
        {
            var c = ReadUInt16();
            return Color.FromArgb((c & 0x1F) << 3, (c >> 5 & 0x1F) << 3, (c >> 10 & 0x1F) << 3);
        }

        /// <summary>
        /// Reads the specified number of BGR555 colors from the stream and advances the position by twice that many bytes.
        /// </summary>
        /// <param name="colors">The number of expected colors in the palette.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">colors was not 16 or 256.</exception>
        public Palette ReadPalette(int colors = 16)
        {
            var pal = new Palette(colors);
            for (int i = 0; i < colors; i++)
                pal[i] = ReadColor();
            return pal;
        }

        /// <summary>
        /// lol reads a compressed palette or something.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">the decompressed bytes were not the correct size (32 or 512).</exception>
        public Palette ReadCompressedPalette()
        {
            var buffer = ReadCompressedBytes();
            var pal = new Palette(buffer.Length / 2);
            for (int i = 0; i < pal.Length; i++)
            {
                var color = (buffer[i * 2 + 1] << 8) | buffer[i * 2];
                pal[i] = Color.FromArgb((color & 0x1F) << 3, (color >> 5 & 0x1F) << 3, (color >> 10 & 0x1F) << 3);
            }
            return pal;
        }

        public Sprite ReadSprite(int tiles, BitDepth bitDepth)
        {
            if (bitDepth == BitDepth.Four)
                return ReadSprite4(tiles);
            else
                return ReadSprite8(tiles);
        }

        public Sprite ReadSprite4(int tiles)
        {
            try
            {
                return Sprite.From4Bpp(ReadBytes(tiles * 32));
            }
            catch
            {
                return null;
            }
        }

        public Sprite ReadSprite8(int tiles)
        {
            try
            {
                return Sprite.From8Bpp(ReadBytes(tiles * 64));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Reads a compressed sprite of the given bit depth.
        /// </summary>
        /// <param name="bitDepth"></param>
        /// <returns>A sprite.</returns>
        public Sprite ReadCompressedSprite(BitDepth bitDepth)
        {
            if (bitDepth == BitDepth.Four)
                return ReadCompressedSprite4();
            else
                return ReadCompressedSprite8();
        }

        /// <summary>
        /// Reads a compressed 4BPP sprite.
        /// </summary>
        /// <returns>A sprite.</returns>
        public Sprite ReadCompressedSprite4()
        {
            try
            {
                return Sprite.From4Bpp(ReadCompressedBytes());
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Reads a compressed 8BPP sprite.
        /// </summary>
        /// <returns></returns>
        public Sprite ReadCompressedSprite8()
        {
            try
            {
                return Sprite.From8Bpp(ReadCompressedBytes());
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Write

        /// <summary>
        /// Writes a 4-byte ROM pointer to the stream and advances the position by four bytes.
        /// </summary>
        /// <param name="offset">The offset to write a pointer to.</param>
        public void WritePointer(int offset)
        {
            if (offset <= 0)
                WriteUInt32(0u);
            else
                WriteUInt32((uint)offset + 0x08000000u);
        }

        public void WriteText(string str, Table.Encoding encoding)
        {
            WriteBytes(Table.GetBytes(str, encoding));
        }

        public void WriteText(string str, int length, Table.Encoding encoding)
        {
            // convert string
            byte[] buffer = Table.GetBytes(str, encoding);

            // ensure proper length
            if (buffer.Length != length)
                Array.Resize(ref buffer, length);
            buffer[length - 1] = 0xFF;

            WriteBytes(buffer);
        }

        public void WriteTextTable(string[] table, int entryLength, Table.Encoding encoding)
        {
            foreach (var str in table)
                WriteText(str, entryLength, encoding);
        }

        public void WriteColor(Color color)
        {
            WriteUInt16((ushort)((color.R / 8) | (color.G / 8 << 5) | (color.B / 3 << 10)));
        }

        public void WritePalette(Palette palette)
        {
            foreach (Color color in palette)
                WriteUInt16((ushort)((color.R / 8) | (color.G / 8 << 5) | (color.B / 3 << 10)));
        }

        public void WriteCompressedPalette(Palette palette)
        {
            // buffer to hold uncompressed color data
            byte[] buffer = new byte[palette.Length * 2];

            // copy colors to buffer
            for (int i = 0; i < palette.Length; i++)
            {
                Color color = palette[i];
                ushort u = (ushort)((color.R / 8) | (color.G / 8 << 5) | (color.B / 3 << 10));

                buffer[i * 2] = (byte)u;
                buffer[i * 2 + 1] = (byte)(u >> 8);
            }

            // write compressed bytes
            WriteCompressedBytes(buffer);
        }

        public void WriteSprite(Sprite sprite)
        {
            if (sprite.BitDepth == BitDepth.Four)
                WriteSprite4(sprite);
            else
                WriteSprite8(sprite);
        }

        public void WriteSprite4(Sprite sprite)
        {
            WriteBytes(sprite.To4Bpp());
        }

        public void WriteSprite8(Sprite sprite)
        {
            WriteBytes(sprite.To8Bpp());
        }

        public void WriteCompressedSprite4(Sprite sprite)
        {
            WriteCompressedBytes(sprite.To4Bpp());
        }

        public void WriteCompressedSprite8(Sprite sprite)
        {
            WriteCompressedBytes(sprite.To8Bpp());
        }

        #endregion

        #region Search

        /// <summary>
        /// Finds and changes all pointers from one offset to another.
        /// </summary>
        /// <param name="oldOffset">The offset to search for.</param>
        /// <param name="newOffset">The offset to replace.</param>
        /// <returns>An <see cref="int"/> array containing the offsets of every repointed pointer.</returns>
        public unsafe int[] Repoint(int oldOffset, int newOffset)
        {
            int oldPointer = oldOffset + 0x08000000;    // pointer to originalOffset
            int newPointer = newOffset + 0x08000000;    // pointer to newOffset
            List<int> result = new List<int>();         // list of offsets that had pointers replaced

            // ------------------------------
            // fastest find/replace I could think of
            // NOTE: assumes little endian system
            byte[] buffer = ReadAllBytes();

            fixed (byte* bufferPtr = &buffer[0])
            {
                for (int i = 0; i < buffer.Length - 4; i++)
                {
                    // get value at i
                    int* ptr = (int*)(bufferPtr + i);

                    // check and replace pointer
                    if (*ptr == oldPointer)
                    {
                        *ptr = newPointer;

                        result.Add(i);
                        i += 3;
                    }
                }
            }

            // write buffer if there were any actual changes
            if (result.Count > 0)
                WriteAllBytes(buffer);

            // return offsets of replaced pointers
            return result.ToArray();
        }

        #endregion

        #region Properties

        // so we aren't continuously re-reading these properties
        private string name, code, maker;

        /// <summary>
        /// Gets the 12 character name of the ROM specified by the header at offset 0xA0.
        /// </summary>
        public string Name
        {
            get
            {
                if (name != null)
                    return name;

                var i = Position;
                Position = 0xA0;
                name = ReadString(12);
                Position = i;

                return name;
            }
        }

        /// <summary>
        /// Gets the 4 character code of the ROM specified by the header at offset 0xAC.
        /// </summary>
        public string Code
        {
            get
            {
                if (code != null)
                    return code;

                var i = Position;
                Position = 0xAC;
                code = ReadString(4);
                Position = i;

                return code;
            }
        }

        /// <summary>
        /// Gets the 2 character maker code of the ROM specified by the header at offset 0xB0.
        /// </summary>
        public string Maker
        {
            get
            {
                if (maker != null)
                    return maker;

                var i = Position;
                Position = 0xB0;
                maker = ReadString(2);
                Position = i;

                return maker;
            }
        }

        #endregion
    }
}

