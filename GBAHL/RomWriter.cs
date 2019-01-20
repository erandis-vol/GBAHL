using System;
using System.IO;
using System.Text;

namespace GBAHL
{
    /// <summary>
    /// Writes primitive types to a stream.
    /// </summary>
    public class RomWriter : IDisposable
    {
        protected const int BufferSize = 8; // tune as needed, 8 is all we need for 64-bit integers

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
            Dispose();
        }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="BinaryStream"/> class.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            _stream.Dispose();
            _buffer = null;
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
        public int Seek(int offset)
        {
            return (int)_stream.Seek(offset, SeekOrigin.Begin);
        }

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
        /// Moves the position of the stream by the specified number of bytes.
        /// </summary>
        /// <param name="bytes">The number of bytes to move; sign determines direction.</param>
        public int Skip(int bytes)
        {
            return (int)_stream.Seek(bytes, SeekOrigin.Current);
        }

        #region Write

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
            // utf8 encoded string
            WriteBytes(Encoding.UTF8.GetBytes(str));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the length of the stream.
        /// </summary>
        public long Length => _stream.Length;

        /// <summary>
        /// Gets or sets the position of the stream.
        /// </summary>
        public long Position
        {
            get => _stream.Position;
            set => _stream.Position = value;
        }

        /// <summary>
        /// Gets whether the stream has read to the end.
        /// </summary>
        public bool EndOfStream => _stream.Position >= _stream.Length;

        #endregion
    }
}
