using System;
using System.Collections.Generic;
using System.IO;
using GBAHL.Text;
using GBAHL.Drawing;

namespace GBAHL.IO
{
    /// <summary>
    /// Reads and writes primitive data types to/from a GBA ROM.
    /// </summary>
    public class GbaBinaryStream : BinaryStream
    {
        private const int CompressionLZSS = 0x10;
        private const int CompressionHuffman = 0x20;
        private const int CompressionRLE = 0x30;

        /// <summary>
        /// Initializes a new instance of the <see cref="GbaBinaryStream"/> class based on the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <exception cref="ArgumentException"><paramref name="stream"/> is not a valid ROM size.</exception>
        public GbaBinaryStream(Stream stream) : base(stream)
        {
#if ENFORCE_ROM_SIZE
            if (Length % 0x1000000 != 0)
            {
                throw new ArgumentException("Stream is not the correct size for a ROM!");
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
        /// If possible, reads compressed data from the stream into a byte array and advances the position.
        /// </summary>
        /// <returns>The decompressed bytes.</returns>
        /// <exception cref="InvalidDataException">the data uses an unsupported compression format or is not compressed.</exception>
        public byte[] ReadCompressedBytes()
        {
            var compressionType = ReadByte();
            if (compressionType == CompressionLZSS)
            {
                var length = ReadInt24();
                var result = new byte[length];

                //var i = 4;
                var j = 0;

                while (j < length)
                {
                    var flags = ReadByte();

                    for (int k = 7; k >= 0 && j < length; k--)
                    {
                        var isCompressed = ((flags >> k) & 1) == 1;
                        if (isCompressed)
                        {
                            var data = (ReadByte() << 8) | ReadByte();

                            var n = (data >> 12) + 3;
                            var disp = j - (data & 0xFFF) - 1;

                            while (n-- > 0 && j < length)
                            {
                                result[j++] = result[disp++];
                            }
                        }
                        else
                        {
                            result[j++] = ReadByte();
                        }
                    }
                }

                return result;
            }
            else
            {
                throw new InvalidDataException($"Unsupported compression type {compressionType:x2}.");
            }
        }

        /// <summary>
        /// If possible, reads compressed data and returns the number of bytes it occupies in the ROM.
        /// </summary>
        /// <returns>The number of bytes the compressed data, or -1 if there is an error.</returns>
        public int ReadCompressedSize()
        {
            var start = Position;

            var compressionType = ReadByte();
            if (compressionType == CompressionLZSS)
            {
                // get decompressed length
                var length = ReadInt24();

                // try to decompress the data
                int size = 0, pos = 0, flags = 0;
                while (size < length)
                {
                    if (Position >= BufferSize)
                        return -1;

                    if (pos == 0)
                        flags = ReadByte();

                    if ((flags & (0x80 >> pos)) == 0)
                    {
                        size++;
                        Skip(1);
                    }
                    else
                    {
                        int block = (ReadByte() << 8) | ReadByte();

                        int bytes = (block >> 12) + 3;
                        int disp = size - (block & 0xFFF) - 1;

                        while (bytes-- > 0 && size < length)
                        {
                            if (disp < 0 || disp >= length)
                                return -1;

                            size++;
                            disp++;
                        }
                    }

                    pos = ++pos % 8;
                }

                return Position - start;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Reads an FF-terminated string using the given <see cref="Encoding"/> and advances the position.
        /// </summary>
        /// <param name="encoding">The encoding of the string.</param>
        /// <returns></returns>
        public string ReadText(Encoding encoding)
        {
            // read string until FF
            List<byte> buffer = new List<byte>();
            byte temp = 0;
            do
            {
                buffer.Add((temp = ReadByte()));
            } while (temp != 0xFF);

            // convert to string
            return encoding.Decode(buffer.ToArray());
        }

        /// <summary>
        /// Reads a string of the given length using the given <see cref="Table.Encoding"/> and advances the position by that many bytes.
        /// </summary>
        /// <param name="length">The length of the string.</param>
        /// <param name="encoding">The encoding of the string.</param>
        /// <returns></returns>
        public string ReadText(int length, Encoding encoding)
        {
            return encoding.Decode(ReadBytes(length));
        }

        /// <summary>
        /// Reads a table of strings of the given length using the given <see cref="Table.Encoding"/> and advances the position by that many bytes.
        /// </summary>
        /// <param name="stringLength">The length of the string.</param>
        /// <param name="tableSize">The length of the table.</param>
        /// <param name="encoding">The encoding of the string.</param>
        /// <returns></returns>
        public string[] ReadTextTable(int stringLength, int tableSize, Encoding encoding)
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
        public Bgr555 ReadColor() => ReadUInt16();

        /// <summary>
        /// Reads the specified number of BGR555 colors from the stream and advances the position by twice that many bytes.
        /// </summary>
        /// <param name="colors">The number of expected colors in the palette.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">colors was not 16 or 256.</exception>
        public Palette ReadPalette(int colors = 16)
        {
            Palette palette = new Palette(Bgr555.Black, colors);
            for (int i = 0; i < colors; i++)
            {
                palette[i] = ReadColor();
            }
            return palette;
        }

        /// <summary>
        /// Reads a compressed palette from the stream.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">the decompressed bytes were not the correct size (32 or 512).</exception>
        public Palette ReadCompressedPalette()
        {
            var buffer = ReadCompressedBytes();

            var palette = new Palette(Bgr555.Black, buffer.Length / 2);
            for (int i = 0; i < palette.Length; i++)
            {
                palette[i] = new Bgr555((ushort)((buffer[i * 2 + 1] << 8) | buffer[i * 2]));
            }

            return palette;
        }

        public Tileset ReadTiles4(int tiles)
        {
            return new Tileset(BitDepth.Decode4(ReadBytes(tiles * 32)));
        }

        public Tileset ReadTiles8(int tiles)
        {
            return new Tileset(BitDepth.Decode8(ReadBytes(tiles * 32)));
        }

        public Tileset ReadCompressedTiles4()
        {
            // TODO: Safety checks
            return new Tileset(BitDepth.Decode4(ReadCompressedBytes()));
        }

        public Tileset ReadCompressedTiles8()
        {
            // TODO: Safety checks
            return new Tileset(BitDepth.Decode8(ReadCompressedBytes()));
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

        /// <summary>
        /// Compresses and writes an array of bytes to the stream and advances the position.
        /// </summary>
        /// <param name="buffer">The <see cref="byte"/> array to compress.</param>
        public void WriteCompressedBytes(byte[] buffer)
        {
            // TODO: Allow choice of compression method
            var compressed = Compression.LZSS.Compress(buffer);
            WriteBytes(compressed);
        }

        public void WriteText(string str, Encoding encoding)
        {
            WriteBytes(encoding.Encode(str));
        }

        public void WriteText(string str, int length, Encoding encoding)
        {
            // convert string
            byte[] buffer = encoding.Encode(str);

            // ensure proper length
            if (buffer.Length != length)
                Array.Resize(ref buffer, length);
            buffer[length - 1] = 0xFF;

            WriteBytes(buffer);
        }

        public void WriteTextTable(string[] table, int entryLength, Encoding encoding)
        {
            foreach (var str in table)
                WriteText(str, entryLength, encoding);
        }

        public void WriteColor(Bgr555 color)
        {
            WriteUInt16(color.ToUInt16());
        }

        public void WritePalette(Palette palette)
        {
            foreach (var color in palette)
            {
                WriteColor(color);
            }
        }

        public void WriteCompressedPalette(Palette palette)
        {
            // Buffer to hold uncompressed color data
            byte[] buffer = new byte[palette.Length * 2];

            // Copy colors to buffer
            for (int i = 0; i < palette.Length; i++)
            {
                var bgr = palette[i].ToUInt16();

                buffer[i * 2] = (byte)bgr;
                buffer[i * 2 + 1] = (byte)(bgr >> 8);
            }

            // Write compressed bytes
            WriteCompressedBytes(buffer);
        }

        public void WriteTiles4(Tileset tiles)
        {
            if (tiles == null)
                throw new ArgumentNullException(nameof(tiles));

            WriteBytes(BitDepth.Encode4(tiles));
        }

        public void WriteTiles4(Tile[] tiles)
        {
            if (tiles == null)
                throw new ArgumentNullException(nameof(tiles));

            WriteBytes(BitDepth.Encode4(tiles));
        }

        public void WriteTiles8(Tileset tiles)
        {
            if (tiles == null)
                throw new ArgumentNullException(nameof(tiles));

            WriteBytes(BitDepth.Encode8(tiles));
        }

        public void WriteTiles8(Tile[] tiles)
        {
            if (tiles == null)
                throw new ArgumentNullException(nameof(tiles));

            WriteBytes(BitDepth.Encode8(tiles));
        }

        public void WriteCompressedTiles4(Tileset tiles)
        {
            if (tiles == null)
                throw new ArgumentNullException(nameof(tiles));

            WriteCompressedBytes(BitDepth.Encode4(tiles));
        }

        public void WriteCompressedTiles4(Tile[] tiles)
        {
            if (tiles == null)
                throw new ArgumentNullException(nameof(tiles));

            WriteCompressedBytes(BitDepth.Encode4(tiles));
        }

        public void WriteCompressedTiles8(Tileset tiles)
        {
            if (tiles == null)
                throw new ArgumentNullException(nameof(tiles));

            WriteCompressedBytes(BitDepth.Encode8(tiles));
        }

        public void WriteCompressedTiles8(Tile[] tiles)
        {
            if (tiles == null)
                throw new ArgumentNullException(nameof(tiles));

            WriteCompressedBytes(BitDepth.Encode8(tiles));
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