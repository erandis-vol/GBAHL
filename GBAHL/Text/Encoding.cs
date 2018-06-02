using System;
using System.Collections.Generic;
using System.IO;

namespace GBAHL.Text
{
    /// <summary>
    /// Represents a character encoding.
    /// </summary>
    public abstract class Encoding
    {
        /// <summary>
        /// Decodes all bytes in the specified array into a string.
        /// </summary>
        /// <param name="bytes">The byte array to be decoded.</param>
        /// <returns>The decoded string.</returns>
        public string Decode(byte[] bytes) => Decode(new ByteReader(bytes));

        /// <summary>
        /// Decodes all bytes in the specified bytes into a string.
        /// </summary>
        /// <param name="bytes">The byte stream to be decoded.</param>
        /// <returns>The decoded string.</returns>
        protected abstract string Decode(ByteReader bytes);

        /// <summary>
        /// Decodes the specified byte into a string.
        /// </summary>
        /// <param name="b">The byte to be decoded.</param>
        /// <returns></returns>
        public abstract string DecodeChar(byte b);

        /// <summary>
        /// Encodes all characters in the specified string into a byte array.
        /// </summary>
        /// <param name="str">The string to be encoded.</param>
        /// <returns>The encoded byte array.</returns>
        /// <exception cref="InvalidDataException"><paramref name="str"/> could not be encoded.</exception>
        public abstract byte[] Encode(string str);

        /// <summary>
        /// Encodes the specified character into a byte array.
        /// </summary>
        /// <param name="ch">The character to be encoded.</param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"><paramref name="ch"/> could not be encoded.</exception>
        public byte EncodeChar(char ch) => EncodeChar(new string(ch, 1));

        /// <summary>
        /// Encodes the specified character into a byte array.
        /// </summary>
        /// <param name="ch">The character to be encoded.</param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"><paramref name="ch"/> could not be encoded.</exception>
        public abstract byte EncodeChar(string ch);

        /// <summary>
        /// Splits the specified string into characters.
        /// </summary>
        /// <param name="str">The string to be split.</param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"><paramref name="str"/> could not be split.</exception>
        public static IEnumerable<string> Split(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];

                if (c == '\\')
                {
                    if (i >= str.Length - 1)
                    {
                        throw new InvalidDataException("Malformed string.");
                    }

                    yield return "\\" + str[++i];
                }
                else if (c == '[')
                {
                    var start = i;

                    // Scan until we reach ']'
                    while (i < str.Length && str[i] != ']')
                    {
                        i++;
                    }

                    // Ensure the string is well-formed
                    if (i >= str.Length)
                    {
                        throw new InvalidDataException("Malformed string.");
                    }

                    yield return str.Substring(start, i - start + 1);
                }
                else
                {
                    yield return new string(c, 1);
                }
            }
        }

        /// <summary>
        /// Represents a read-only stream of bytes.
        /// </summary>
        protected sealed class ByteReader
        {
            private byte[] bytes;

            /// <summary>
            /// Initializes a new instance of the <see cref="ByteReader"/> class.
            /// </summary>
            /// <param name="bytes">The bytes to be streamed.</param>
            /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is <c>null</c>.</exception>
            public ByteReader(byte[] bytes)
            {
                this.bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
            }

            /// <summary>
            /// Reads the next byte from the stream and advances the position by one byte.
            /// </summary>
            /// <returns>The next byte from the stream.</returns>
            /// <exception cref="EndOfStreamException">attempted to read beyond the stream.</exception>
            public byte ReadByte()
            {
                return Position < bytes.Length ? bytes[Position++] : throw new EndOfStreamException();
            }

            /// <summary>
            /// Reads the next byte from the stream but does not advance the position.
            /// </summary>
            /// <returns>The next byte from the stream.</returns>
            /// <exception cref="EndOfStreamException">attempted to read beyond the stream.</exception>
            public byte PeekByte()
            {
                return Position < bytes.Length ? bytes[Position] : throw new EndOfStreamException();
            }

            /// <summary>
            /// Reads the next byte from the stream but does not advance the position.
            /// </summary>
            /// <param name="value">The next byte to be read from the stream.</param>
            /// <returns><c>true</c> if the next byte could be read; otherwise, <c>false</c>.</returns>
            public bool TryPeekByte(out byte value)
            {
                if (Position < bytes.Length)
                {
                    value = bytes[Position];
                    return true;
                }

                value = 0;
                return false;
            }

            /// <summary>
            /// Gets or sets the position of the stream.
            /// </summary>
            public int Position { get; set; } = 0;

            /// <summary>
            /// Gets the length of the stream in bytes.
            /// </summary>
            public int Length => bytes.Length;

            /// <summary>
            /// Determines whether the stream has more bytes to read.
            /// </summary>
            public bool HasMore => Position < Length;
        }

        /// <summary>
        /// Represents a write-only stream of bytes.
        /// </summary>
        protected sealed class ByteWriter
        {
            private List<byte> bytes;

            /// <summary>
            /// Initializes a new instance of the <see cref="ByteWriter"/> class.
            /// </summary>
            public ByteWriter()
            {
                bytes = new List<byte>();
            }

            /// <summary>
            /// Writes the specified byte value to the stream.
            /// </summary>
            /// <param name="value">The value to be written.</param>
            public void Write(byte value)
            {
                bytes.Add(value);
            }

            /// <summary>
            /// Copies the stream to a new byte array.
            /// </summary>
            /// <returns></returns>
            public byte[] ToArray() => bytes.ToArray();

            /// <summary>
            /// Gets the length of the stream in bytes.
            /// </summary>
            public int Length => bytes.Count;
        }
    }
}
