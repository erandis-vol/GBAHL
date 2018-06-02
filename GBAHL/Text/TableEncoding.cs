using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GBAHL.Text
{
    /// <summary>
    /// Represents a table-based character encoding.
    /// </summary>
    public abstract class TableEncoding : Encoding
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableEncoding"/> class for the specified table.
        /// </summary>
        /// <param name="table">The encoding table.</param>
        protected TableEncoding(Table table)
        {
            Table = table;
        }

        protected override string Decode(ByteReader bytes)
        {
            var sb = new StringBuilder();

            while (bytes.HasMore)
            {
                sb.Append(DecodeChar(bytes.ReadByte()));
            }

            return sb.ToString();
        }

        public override string DecodeChar(byte b)
        {
            return Table.GetCharacter(b);
        }

        public override byte[] Encode(string str)
        {
            var bytes = new List<byte>();

            foreach (var ch in Split(str))
            {
                bytes.Add(EncodeChar(ch));
            }

            return bytes.ToArray();
        }

        public override byte EncodeChar(string ch)
        {
            var value = Table.GetByte(ch);

            if (value == -1)
                throw new InvalidDataException($"Could not encode '{ch}'.");

            return (byte)value;
        }

        /// <summary>
        /// Gets the encoding table.
        /// </summary>
        public Table Table { get; }
    }
}
