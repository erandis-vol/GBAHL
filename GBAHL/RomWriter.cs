using GBAHL.Drawing;
using GBAHL.Text;
using System;
using System.Diagnostics;
using System.IO;

namespace GBAHL
{
    /// <summary>
    /// Writes primitive types to a stream.
    /// </summary>
    public class RomWriter : IDisposable
    {
        protected const int BufferSize = 8;

        private Stream _stream;
        private byte[] _buffer = new byte[BufferSize];
        private bool _leaveOpen;
        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="RomWriter"/> class based on the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <exception cref="FileNotFoundException"></exception>
        public RomWriter(RomFileInfo file)
            : this(file.OpenWrite(), false)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RomWriter"/> class based on the specified stream.
        /// </summary>
        /// <param name="stream">The input stream.</param>
        public RomWriter(Stream stream)
            : this(stream, false)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RomWriter"/> class based on the specified stream,
        /// and optionally leaves the stream open.
        /// </summary>
        /// <param name="stream">The input stream.</param>
        /// <param name="leaveOpen">
        /// true to leave the stream open after the <see cref="RomWriter"/> object is disposed; otherwise, false.
        /// </param>
        public RomWriter(Stream stream, bool leaveOpen)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _buffer = new byte[BufferSize];
            _leaveOpen = leaveOpen;
            _isDisposed = false;
        }

        ~RomWriter()
        {
            Dispose(false);
        }

        #region Methods

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="RomWriter"/> class.
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
                throw new ObjectDisposedException(nameof(RomWriter));
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

        /// <summary>
        /// Writes a byte to the stream and advances the position by one byte.
        /// </summary>
        /// <param name="value">The <see cref="byte"/> value to write.</param>
        public void WriteByte(byte value)
        {
            _stream.WriteByte(value);
        }

        /// <summary>
        /// Writes a signed byte to the stream and advances the position by one byte.
        /// </summary>
        /// <param name="value">The <see cref="sbyte"/> value to write.</param>
        public void WriteSByte(sbyte value)
        {
            _stream.WriteByte((byte)value);
        }

        /// <summary>
        /// Writes a 2-byte unsigned integer to the stream and advances the position by two bytes.
        /// </summary>
        /// <param name="value">The <see cref="ushort"/> value to write.</param>
        public void WriteUInt16(ushort value)
        {
            _buffer[0] = (byte)value;
            _buffer[1] = (byte)(value >> 8);
            _stream.Write(_buffer, 0, 2);
        }

        /// <summary>
        /// Writes a 4-byte signed integer to the stream and advances the position by four bytes.
        /// </summary>
        /// <param name="value">The <see cref="int"/> value to write.</param>
        public void WriteInt32(int value)
        {
            _buffer[0] = (byte)value;
            _buffer[1] = (byte)(value >> 8);
            _buffer[2] = (byte)(value >> 16);
            _buffer[3] = (byte)(value >> 24);
            _stream.Write(_buffer, 0, 4);
        }

        /// <summary>
        /// Writes a 4-byte unsigned integer to the stream and advances the position by four bytes.
        /// </summary>
        /// <param name="value">The <see cref="uint"/> value to write.</param>
        public void WriteUInt32(uint value)
        {
            _buffer[0] = (byte)value;
            _buffer[1] = (byte)(value >> 8);
            _buffer[2] = (byte)(value >> 16);
            _buffer[3] = (byte)(value >> 24);
            _stream.Write(_buffer, 0, 4);
        }

        /// <summary>
        /// Writes an 8-byte unsigned integer to the stream and advances the position by eight bytes.
        /// </summary>
        /// <param name="value">The <see cref="ulong"/> value to write.</param>
        public void WriteUInt64(ulong value)
        {
            _buffer[0] = (byte)value;
            _buffer[1] = (byte)(value >> 8);
            _buffer[2] = (byte)(value >> 16);
            _buffer[3] = (byte)(value >> 24);
            _buffer[3] = (byte)(value >> 32);
            _buffer[1] = (byte)(value >> 40);
            _buffer[2] = (byte)(value >> 48);
            _buffer[3] = (byte)(value >> 56);
            _stream.Write(_buffer, 0, 8);
        }

        /// <summary>
        /// Writes a <see cref="byte"/> array to the stream and advances the position by the length of the array.
        /// </summary>
        /// <param name="bytes">The <see cref="byte"/> array to write.</param>
        /// <exception cref="ArgumentNullException">bytes is null.</exception>
        public void WriteBytes(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");

            _stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Writes a <see cref="byte"/> to the stream the specified number of times and advances the position by that many bytes.
        /// </summary>
        /// <param name="value">The <see cref="byte"/> value to write.</param>
        /// <param name="count">The number of times to write the value.</param>
        public void WriteBytes(byte value, int count)
        {
            for (int i = 0; i < count; i++)
                _stream.WriteByte(value);
        }

        /// <summary>
        /// Writes a UTF-8 encoded string to the stream and advances the position.
        /// </summary>
        /// <param name="str">The <see cref="string"/> value to write.</param>
        public void WriteString(string str)
        {
            // TODO: We can avoid allocation here.
            WriteBytes(System.Text.Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// Writes a 4-byte pointer to the stream and advances the position by four bytes.
        /// </summary>
        /// <param name="value">The <see cref="Ptr"/> value to write.</param>
        /// <exception cref="ArgumentException"><paramref name="value"/> is invalid.</exception>
        public void WritePtr(Ptr value)
        {
            if (value.IsZero)
            {
                WriteInt32(0);
            }
            else if (value.IsValid)
            {
                WriteInt32((value.Bank << 24) | value.Address);
            }
            else
            {
                throw new ArgumentException("Cannot write an invalid pointer.", nameof(value));
            }
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
