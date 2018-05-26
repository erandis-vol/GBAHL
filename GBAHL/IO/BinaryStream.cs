using System;
using System.IO;
using System.Text;

namespace GBAHL.IO
{
    /// <summary>
    /// Reads and writes primitive data types to/from a <see cref="Stream"/>.
    /// </summary>
    public class BinaryStream : IDisposable
    {
        private const int BUFFER_SIZE = 8; // tune as needed, 8 is all we need for 64-bit integers
        private byte[] buffer = new byte[BUFFER_SIZE];

        private Stream stream;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryStream"/> class based on the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <exception cref="ArgumentException"><paramref name="stream"/> cannot be read from or written to.</exception>
        public BinaryStream(Stream stream)
        {
            if (!stream.CanRead || !stream.CanWrite)
                throw new ArgumentException("stream", "Stream is not read-write!");

            this.stream = stream;
        }

        ~BinaryStream()
        {
            Dispose();
        }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="BinaryStream"/> class.
        /// </summary>
        public void Dispose()
        {
            if (disposed) return;
            disposed = true;

            stream.Dispose();
            buffer = null;
        }

        /// <summary>
        /// Forces data to be written to the underlying disk.
        /// </summary>
        /// <exception cref="IOException"></exception>
        public void Flush()
        {
            stream.Flush();
        }

        /// <summary>
        /// Sets the position of the stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to 0.</param>
        public int Seek(int offset)
        {
            return (int)stream.Seek(offset, SeekOrigin.Begin);
        }

        /// <summary>
        /// Sets the position of the stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        public int Seek(int offset, SeekOrigin origin)
        {
            return (int)stream.Seek(offset, origin);
        }

        /// <summary>
        /// Moves the position of the stream by the specified number of bytes.
        /// </summary>
        /// <param name="bytes">The number of bytes to move; sign determines direction.</param>
        public int Skip(int bytes)
        {
            return (int)stream.Seek(bytes, SeekOrigin.Current);
        }

        // safely fill the buffer from the stream
        protected void FillBuffer(int bytes)
        {
            if (bytes <= 0 || bytes > stream.Length)
                throw new Exception();

            if (bytes == 1)
            {
                int n = stream.ReadByte();
                if (n == -1)
                    throw new EndOfStreamException();

                buffer[0] = (byte)n;
            }
            else
            {
                int bytesRead = 0;

                do
                {
                    int n = stream.Read(buffer, bytesRead, bytes - bytesRead);
                    if (n == 0)
                        throw new EndOfStreamException();
                    bytesRead += n;
                } while (bytesRead < bytes);
            }
        }

        // safely read the entire stream into a buffer
        protected byte[] ReadAllBytes()
        {
            // preserve original position
            long originalPosition = stream.Position;

            // seek start and read entire file
            byte[] buffer = new byte[stream.Length];
            int bytesRead = 0;

            stream.Seek(0, SeekOrigin.Begin);
            do
            {
                int n = stream.Read(buffer, bytesRead, buffer.Length - bytesRead);
                if (n == 0)
                    throw new EndOfStreamException("Unable to read entire ROM!");
                bytesRead += n;
            } while (bytesRead < buffer.Length);

            // reset current position
            stream.Position = originalPosition;

            // done
            return buffer;
        }

        // safe write the entire stream from a buffer
        protected void WriteAllBytes(byte[] buffer)
        {
            // preserve original position
            long originalPosition = stream.Position;

            // overwrite entire stream
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(buffer, 0, buffer.Length);

            // reset position
            stream.Position = originalPosition;
        }

        #region Read

        /// <summary>
        /// Reads the next byte from the stream and advances the position by one byte.
        /// </summary>
        /// <exception cref="EndOfStreamException">attempted to read beyond the stream.</exception>
        /// <returns>The next byte read from the stream.</returns>
        public byte ReadByte()
        {
            int n = stream.ReadByte();
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
            int n = stream.ReadByte();
            if (n == -1)
                throw new EndOfStreamException();

            stream.Position -= 1;
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
            return (ushort)(buffer[0] | (buffer[1] << 8));
        }

        /// <summary>
        /// Reads a 4-byte signed integer from the stream and advances the position by four bytes.
        /// </summary>
        /// <returns></returns>
        public int ReadInt32()
        {
            FillBuffer(4);
            return buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24);
        }

        /// <summary>
        /// Reads a 4-byte unsigned integer from the stream and advances the position by four bytes.
        /// </summary>
        /// <returns></returns>
        public uint ReadUInt32()
        {
            FillBuffer(4);
            return (uint)(buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24));
        }

        /// <summary>
        /// Reads an 8-byte unsigned integer from the stream and advances the position by eight bytes.
        /// </summary>
        /// <returns></returns>
        public ulong ReadUInt64()
        {
            FillBuffer(8);
            return (ulong)(buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24) |
                (buffer[4] << 32) | (buffer[5] << 40) | (buffer[6] << 48) | (buffer[7] << 56));
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
                int n = stream.Read(result, bytesRead, count - bytesRead);
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
        /// If possible, reads LZSS compressed bytes from the stream into a byte array and advances the position.
        /// </summary>
        /// <returns>The decompressed bytes.</returns>
        public byte[] ReadCompressedBytes()
        {
            // check if actually compressed
            if (!PeekCompressed())
                return new byte[0];

            // skip L7ZZ identifier
            Skip(1);

            // read decompressed size
            var length = ReadInt24();
            var buffer = new byte[length];

            // decompress the data
            int size = 0, 
                pos = 0, 
                flags = 0;

            while (size < length)
            {
                if (pos == 0) flags = ReadByte();

                if ((flags & (0x80 >> pos)) == 0)
                {
                    // read value to buffer
                    buffer[size++] = ReadByte();
                }
                else
                {
                    // copy block from buffer
                    int block = (ReadByte() << 8) | ReadByte();

                    int bytes = (block >> 12) + 3;
                    int disp = size - (block & 0xFFF) - 1;

                    while (bytes-- > 0 && size < length)
                    {
                        buffer[size++] = buffer[disp++];
                    }
                }

                pos = ++pos % 8;
            }

            return buffer;
        }

        /// <summary>
        /// If possible, reads LZSS compressed data and returns the number of bytes it occupied in the ROM.
        /// </summary>
        /// <returns></returns>
        public int ReadCompressedSize()
        {
            int start = Position;

            // check for compression identifier
            if (ReadByte() != 0x10)
                return -1;

            // get decompressed length
            var length = ReadInt24();

            // try to decompress the data
            int size = 0, pos = 0, flags = 0;
            while (size < length)
            {
                if (Position >= buffer.Length)
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

        /// <summary>
        /// Returns whether the following data could be L7ZZ compressed.
        /// </summary>
        /// <returns><c>true</c> if it could be; <c>false</c> otherwise.</returns>
        public bool PeekCompressed()
        {
            return PeekByte() == 0x10;
        }

        /// <summary>
        /// Reads a 3-byte unsigned integer from the stream and advances the position by three bytes.
        /// </summary>
        /// <returns></returns>
        protected int ReadInt24()
        {
            FillBuffer(3);
            return buffer[0] | (buffer[1] << 8) | (buffer[2] << 16);
        }

        #endregion

        #region Write

        /// <summary>
        /// Writes a byte to the stream and advances the position by one byte.
        /// </summary>
        /// <param name="value">The <see cref="byte"/> value to write.</param>
        public void WriteByte(byte value)
        {
            stream.WriteByte(value);
        }

        /// <summary>
        /// Writes a signed byte to the stream and advances the position by one byte.
        /// </summary>
        /// <param name="value">The <see cref="sbyte"/> value to write.</param>
        public void WriteSByte(sbyte value)
        {
            stream.WriteByte((byte)value);
        }

        /// <summary>
        /// Writes a 2-byte unsigned integer to the stream and advances the position by two bytes.
        /// </summary>
        /// <param name="value">The <see cref="ushort"/> value to write.</param>
        public void WriteUInt16(ushort value)
        {
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            stream.Write(buffer, 0, 2);
        }

        /// <summary>
        /// Writes a 4-byte signed integer to the stream and advances the position by four bytes.
        /// </summary>
        /// <param name="value">The <see cref="int"/> value to write.</param>
        public void WriteInt32(int value)
        {
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);
            stream.Write(buffer, 0, 4);
        }

        /// <summary>
        /// Writes a 4-byte unsigned integer to the stream and advances the position by four bytes.
        /// </summary>
        /// <param name="value">The <see cref="uint"/> value to write.</param>
        public void WriteUInt32(uint value)
        {
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);
            stream.Write(buffer, 0, 4);
        }

        /// <summary>
        /// Writes an 8-byte unsigned integer to the stream and advances the position by eight bytes.
        /// </summary>
        /// <param name="value">The <see cref="ulong"/> value to write.</param>
        public void WriteUInt64(ulong value)
        {
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);
            buffer[3] = (byte)(value >> 32);
            buffer[1] = (byte)(value >> 40);
            buffer[2] = (byte)(value >> 48);
            buffer[3] = (byte)(value >> 56);
            stream.Write(buffer, 0, 8);
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

            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Writes a <see cref="byte"/> to the stream the specified number of times and advances the position by that many bytes.
        /// </summary>
        /// <param name="value">The <see cref="byte"/> value to write.</param>
        /// <param name="count">The number of times to write the value.</param>
        public void WriteBytes(byte value, int count)
        {
            for (int i = 0; i < count; i++)
                stream.WriteByte(value);
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

        /// <summary>
        /// Compresses and writes an array of bytes to the stream and advances the position.
        /// </summary>
        /// <param name="buffer">The <see cref="byte"/> array to compress.</param>
        public void WriteCompressedBytes(byte[] buffer)
        {
            // compress buffer
            byte[] bytes = Compression.LZSS.Compress(buffer);

            // write buffer to ROM
            stream.Write(bytes, 0, bytes.Length);
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

        public int Find(byte search, int count, int startOffset = 0)
        {
            if (startOffset < 0 || startOffset > stream.Length - count)
                return -1;

            if (count <= 0)
                return startOffset;

            byte[] buffer = ReadAllBytes();
            for (int i = startOffset; i < buffer.Length - count; i++)
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

            return -1;
        }

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

        #endregion

        // copy a block of data from one offset to another
        private void MoveBlock(int origin, int destination, int length)
        {
            Seek(origin);
            byte[] buffer = ReadBytes(length);

            Seek(origin);
            WriteBytes(0xFF, length);

            Seek(destination);
            WriteBytes(buffer);
        }

        /// <summary>
        /// Returns whether the given integer is a valid ROM offset.
        /// </summary>
        /// <param name="offset">The integer to validate.</param>
        /// <returns><c>true</c> if the offset is valid; <c>false</c> otherwise.</returns>
        public bool IsValidOffset(int offset)
        {
            return offset >= 0 && offset <= (stream.Length - 1);
        }

        #region Properties

        /// <summary>
        /// Gets the length of the stream.
        /// </summary>
        public int Length
        {
            get { return (int)stream.Length; }
        }

        /// <summary>
        /// Gets or sets the position of the stream.
        /// </summary>
        public int Position
        {
            get { return (int)stream.Position; }
            set { stream.Position = value; }
        }

        /// <summary>
        /// Gets whether the stream has read to the end.
        /// </summary>
        public bool EndOfStream
        {
            get { return stream.Position >= stream.Length; }
        }

        #endregion
    }
}
