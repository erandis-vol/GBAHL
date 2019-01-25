using GBAHL.Drawing;
using GBAHL.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace GBAHL
{
    using StringBuilder = System.Text.StringBuilder;

    /// <summary>
    /// Reads primitive data types from a stream.
    /// </summary>
    public class RomReader : IDisposable
    {
        private const int BufferSize = 8;

        private const int CompressionLZSS    = 0x10;
        private const int CompressionHuffman = 0x20;
        private const int CompressionRLE     = 0x30;

        private Stream _stream;
        private byte[] _buffer;
        private bool _leaveOpen;
        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="RomReader"/> class based on the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <exception cref="FileNotFoundException"></exception>
        public RomReader(RomFileInfo file)
            : this(file.OpenRead(), false)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RomReader"/> class based on the specified stream.
        /// </summary>
        /// <param name="stream">The input stream.</param>
        public RomReader(Stream stream)
            : this(stream, false)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RomReader"/> class based on the specified stream,
        /// and optionally leaves the stream open.
        /// </summary>
        /// <param name="stream">The input stream.</param>
        /// <param name="leaveOpen">
        /// true to leave the stream open after the <see cref="RomReader"/> object is disposed; otherwise, false.
        /// </param>
        public RomReader(Stream stream, bool leaveOpen)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _buffer = new byte[BufferSize];
            _leaveOpen = leaveOpen;
            _isDisposed = false;
        }

        ~RomReader()
        {
            Dispose(false);
        }

        #region Methods

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="RomReader"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (!_leaveOpen)
                {
                    _stream?.Dispose();
                    _stream = null;
                }

                _isDisposed = true;
            }
        }

        [DebuggerNonUserCode]
        protected void AssertNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(RomReader));
            }
        }

        /// <summary>
        /// Forces data to be written to the underlying disk.
        /// </summary>
        /// <exception cref="IOException"></exception>
        public void Flush()
        {
            _stream.Flush();
        }

        /// <summary>
        /// Sets the position of the stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to 0.</param>
        public int Seek(int offset) => Seek(offset, SeekOrigin.Begin);

        /// <summary>
        /// Sets the position of the stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        public int Seek(int offset, SeekOrigin origin)
        {
            return (int)_stream.Seek(offset, origin);
        }

        /// <summary>
        /// Sets the position of the stream.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public int Seek(Ptr offset) => Seek(offset, SeekOrigin.Begin);

        /// <summary>
        /// Sets the position of the stream.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public int Seek(Ptr offset, SeekOrigin origin)
        {
            if (offset.IsValid)
            {
                return (int)_stream.Seek(offset.Address, origin);
            }

            return (int)_stream.Position;
        }

        /// <summary>
        /// Moves the position of the stream by the specified number of bytes.
        /// </summary>
        /// <param name="bytes">The number of bytes to move; sign determines direction.</param>
        public int Skip(int bytes)
        {
            return (int)_stream.Seek(bytes, SeekOrigin.Current);
        }

        // safely fill the buffer from the stream
        protected void FillBuffer(int bytes)
        {
            if (bytes <= 0 || bytes > _stream.Length)
                throw new Exception();

            if (bytes == 1)
            {
                int n = _stream.ReadByte();
                if (n == -1)
                    throw new EndOfStreamException();

                _buffer[0] = (byte)n;
            }
            else
            {
                int bytesRead = 0;

                do
                {
                    int n = _stream.Read(_buffer, bytesRead, bytes - bytesRead);
                    if (n == 0)
                        throw new EndOfStreamException();
                    bytesRead += n;
                } while (bytesRead < bytes);
            }
        }

        /// <summary>
        /// Reads the next byte from the stream and advances the position by one byte.
        /// </summary>
        /// <exception cref="EndOfStreamException">attempted to read beyond the stream.</exception>
        /// <returns>The next byte read from the stream.</returns>
        public byte ReadByte()
        {
            int n = _stream.ReadByte();
            if (n == -1)
                throw new EndOfStreamException();

            return (byte)n;
        }

        /// <summary>
        /// Reads the next byte from the stream.
        /// </summary>
        /// <exception cref="EndOfStreamException">attempted to read beyond the stream.</exception>
        /// <returns>The next byte from the stream.</returns>
        public byte PeekByte()
        {
            int n = _stream.ReadByte();
            if (n == -1)
                throw new EndOfStreamException();

            _stream.Position -= 1;
            return (byte)n;
        }

        /// <summary>
        /// Reads a signed byte from the stream and advances the position by one byte.
        /// </summary>
        /// <returns>The next signed byte read from the stream.</returns>
        public sbyte ReadSByte()
        {
            return (sbyte)ReadByte();
        }

        /// <summary>
        /// Reads a 2-byte unsigned integer from the stream and advances the position by two bytes.
        /// </summary>
        /// <returns>A 2-byte unsigned integer read from the stream.</returns>
        public ushort ReadUInt16()
        {
            FillBuffer(2);
            return (ushort)(_buffer[0] | (_buffer[1] << 8));
        }

        /// <summary>
        /// Reads a 3-byte unsigned integer from the stream and advances the position by three bytes.
        /// </summary>
        /// <returns></returns>
        public int ReadUInt24()
        {
            FillBuffer(3);
            return _buffer[0] | (_buffer[1] << 8) | (_buffer[2] << 16);
        }

        /// <summary>
        /// Reads a 4-byte signed integer from the stream and advances the position by four bytes.
        /// </summary>
        /// <returns></returns>
        public int ReadInt32()
        {
            FillBuffer(4);
            return _buffer[0] | (_buffer[1] << 8) | (_buffer[2] << 16) | (_buffer[3] << 24);
        }

        /// <summary>
        /// Reads a 4-byte unsigned integer from the stream and advances the position by four bytes.
        /// </summary>
        /// <returns></returns>
        public uint ReadUInt32()
        {
            FillBuffer(4);
            return (uint)(_buffer[0] | (_buffer[1] << 8) | (_buffer[2] << 16) | (_buffer[3] << 24));
        }

        /// <summary>
        /// Reads an 8-byte unsigned integer from the stream and advances the position by eight bytes.
        /// </summary>
        /// <returns></returns>
        public ulong ReadUInt64()
        {
            FillBuffer(8);
            return (ulong)(_buffer[0] | (_buffer[1] << 8) | (_buffer[2] << 16) | (_buffer[3] << 24) |
                (_buffer[4] << 32) | (_buffer[5] << 40) | (_buffer[6] << 48) | (_buffer[7] << 56));
        }

        /// <summary>
        /// Reads the specified number of bytes from the stream into a byte array and advances the position by that number of bytes.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="EndOfStreamException"></exception> 
        /// <returns></returns>
        public byte[] ReadBytes(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "Argument cannot be negative!");

            if (count == 0)
                return new byte[0];

            byte[] result = new byte[count];
            int bytesRead = 0;
            do
            {
                int n = _stream.Read(result, bytesRead, count - bytesRead);
                if (n == 0)
                    throw new EndOfStreamException();
                bytesRead += n;
            } while (bytesRead < count);

            return result;
        }

        /// <summary>
        /// Reads a UTF-8 encoded string of the specified length from the stream and advances the position by that number of bytes.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string ReadString(int length, bool breakOnNull = true)
        {
            StringBuilder sb = new StringBuilder();

            byte[] bytes = ReadBytes(length);
            foreach (var b in bytes)
            {
                if (b != 0)
                {
                    sb.Append((char)b);
                }
                else if (breakOnNull)
                {
                    break;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Reads a 4-byte pointer from the stream and advances the position by four bytes.
        /// </summary>
        /// <returns></returns>
        public Ptr ReadPtr()
        {
            const int RomBankStart = 0x08000000;
            const int RomBankEnd   = 0x0AFFFFFF;

            int value = ReadInt32();
            if (value == 0)
            {
                return Ptr.Zero;
            }
            else if (value < RomBankStart || value > RomBankEnd)
            {
                return Ptr.Invalid;
            }
            else
            {
                return (Ptr)(value - RomBankStart);
            }
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
                var length = ReadUInt24();
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
                int length = ReadUInt24();

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
            return new Tileset(BitDepth.Decode8(ReadBytes(tiles * 64)));
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

        public int Find(byte search, int count, int from = 0, int alignment = 1)
        {
            if (from < 0 || from >= _stream.Length - count)
                throw new ArgumentException("", nameof(from));

            if (count <= 0 || count > _stream.Length)
                throw new ArgumentException("", nameof(search));

            if (alignment <= 0)
                alignment = 1;

            if (from % alignment != 0)
                from -= from % alignment;

            int streamPosition = (int)_stream.Position;
            int i = from;
            int j = -1;
            byte[] buffer = new byte[count];

            while (i < _stream.Length)
            {
                _stream.Position = i;

                // Update the search buffer
                FillSearchBuffer(buffer);

                // Check for a match
                bool match = true;
                foreach (byte b in buffer)
                {
                    if (b != search)
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    j = i;
                    break;
                }

                i += alignment;
            }

            _stream.Position = streamPosition;
            return j;
        }

        public int Find(byte[] search, int from = 0, int alignment = 1)
        {
            if (search == null)
                throw new ArgumentNullException(nameof(search));

            if (search.Length > _stream.Length)
                throw new ArgumentException("", nameof(search));

            if (from < 0 || from >= _stream.Length - search.Length)
                throw new ArgumentException("", nameof(from));

            if (alignment <= 0)
                alignment = 1;

            if (from % alignment != 0)
                from -= from % alignment;

            int streamPosition = (int)_stream.Position;
            int i = from;
            int j = -1;
            byte[] buffer = new byte[search.Length];

            while (i < _stream.Length - search.Length)
            {
                _stream.Position = i;

                // Update the search buffer.
                FillSearchBuffer(buffer);

                // Check for a match.
                if (UnsafeCompare(search, buffer))
                {
                    j = i;
                    break;
                }

                i += alignment;
            }

            _stream.Position = streamPosition;
            return j;
        }

        private void FillSearchBuffer(byte[] buffer)
        {
            if (buffer.Length == 1)
            {
                int n = _stream.ReadByte();
                if (n == -1)
                    throw new EndOfStreamException();

                buffer[0] = (byte)n;
            }
            else
            {
                int bytesRead = 0;

                do
                {
                    int n = _stream.Read(buffer, bytesRead, buffer.Length - bytesRead);
                    if (n == 0)
                        throw new EndOfStreamException();
                    bytesRead += n;
                } while (bytesRead < buffer.Length);
            }
        }

        // Copyright (c) 2008-2013 Hafthor Stefansson
        // Distributed under the MIT/X11 software license
        // Ref: http://www.opensource.org/licenses/mit-license.php.
        private static unsafe bool UnsafeCompare(byte[] a1, byte[] a2)
        {
            if (a1 == a2) return true;
            if (a1 == null || a2 == null || a1.Length != a2.Length)
                return false;
            fixed (byte* p1 = a1, p2 = a2)
            {
                byte* x1 = p1, x2 = p2;
                int l = a1.Length;
                for (int i = 0; i < l / 8; i++, x1 += 8, x2 += 8)
                    if (*((long*)x1) != *((long*)x2)) return false;
                if ((l & 4) != 0) { if (*((int*)x1) != *((int*)x2)) return false; x1 += 4; x2 += 4; }
                if ((l & 2) != 0) { if (*((short*)x1) != *((short*)x2)) return false; x1 += 2; x2 += 2; }
                if ((l & 1) != 0) if (*((byte*)x1) != *((byte*)x2)) return false;
                return true;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the length of the stream.
        /// </summary>
        public int Length => (int)_stream.Length;

        /// <summary>
        /// Gets or sets the position of the stream.
        /// </summary>
        public int Position
        {
            get => (int)_stream.Position;
            set => _stream.Position = value;
        }

        /// <summary>
        /// Gets whether the stream has read to the end.
        /// </summary>
        public bool EndOfStream => _stream.Position >= _stream.Length;

        #endregion
    }
}
