using System;
using System.IO;
using System.Text;

namespace GBAHL
{
    /// <summary>
    /// Reads primitive data types from a stream.
    /// </summary>
    public class RomReader : IDisposable
    {
        private const int BufferSize = 8;

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
        {

        }

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

        #region Read

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
        public string ReadString(int length)
        {
            //return Encoding.UTF8.GetString(ReadBytes(length));
            var buffer = ReadBytes(length);
            var sb = new StringBuilder();

            foreach (var b in buffer)
            {
                if (b == 0)
                    break;

                sb.Append((char)b);
            }

            return sb.ToString();
        }



        /// <summary>
        /// Reads a 3-byte unsigned integer from the stream and advances the position by three bytes.
        /// </summary>
        /// <returns></returns>
        protected int ReadInt24()
        {
            FillBuffer(3);
            return _buffer[0] | (_buffer[1] << 8) | (_buffer[2] << 16);
        }

        #endregion

        #region Search

        // these search methds are bad
        /*
        public int FindFreeSpace(int length, byte freespace = 0xFF, int startOffset = 0, int alignment = 1)
        {
            if (alignment <= 0)
                throw new ArgumentOutOfRangeException("alignment");

            if (startOffset < 0 || startOffset > stream.Length)
                throw new ArgumentOutOfRangeException("startOffset");

            // preserve stream position
            var pos = stream.Position;

            // adjust start offset for alignment
            if (alignment > 1 && startOffset % alignment != 0)
                startOffset += alignment - (startOffset % alignment);

            // search ROM for freespace
            int match = -1;
            for (int i = startOffset; i < stream.Length - length; i++)
            {
                stream.Seek(i, SeekOrigin.Begin);
                match = i;

                // check for length bytes of freespace
                for (int j = 0; j < length; j++)
                {
                    if (stream.ReadByte() != freespace)
                    {
                        match = -1;
                        break;
                    }
                }

                if (match != -1)
                    break;
            }

            // return to original stream position
            stream.Seek(pos, SeekOrigin.Begin);

            // return match (or not)
            return match;
        }
        */

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

            /*
            byte[] buffer = ReadAllBytes();
            for (int i = from; i < buffer.Length - count; i++)
            {
                int j = 0;
                while (buffer[i + j] == search && j < count)
                {
                    j++;
                }

                if (j == count)
                {
                    return i;
                }
            }
            */

            _stream.Position = streamPosition;
            return j;
        }

        /*
        public int Find(byte[] search, int startOffset = 0)
        {
            if (startOffset >= stream.Length - search.Length)
                throw new Exception("Bad search start offset.");

            if (search == null || search.Length == 0)
                return startOffset;

            int match = -1;
            var pos = stream.Position;

            for (int i = startOffset; i < stream.Length - search.Length; i++)
            {
                Seek(i);
                byte[] buffer = ReadBytes(search.Length);

                bool b = true;
                for (int j = 0; j < search.Length; j++)
                {
                    if (search[j] != buffer[j])
                    {
                        b = false;
                        break;
                    }
                }

                if (b)
                {
                    match = i;
                    break;
                }
            }

            stream.Position = pos;
            return match;
        }
        */

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
