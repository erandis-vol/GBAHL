using System;
using System.IO;
using System.Linq;
using System.Text;

namespace GBAHL.Text
{
    /// <summary>
    /// Represents a table-based encoding of characters.
    /// </summary>
    public abstract class TableEncoding : Encoding
    {
        private Table table;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableEncoding"/> class
        /// for the specified table.
        /// </summary>
        /// <param name="table">The table to encode.</param>
        internal TableEncoding(Table table)
        {
            this.table = table;
        }

        #region Methods

        public override int GetByteCount(char[] chars, int index, int count)
        {
            if (chars == null)
                throw new ArgumentNullException("chars");

            if (index < 0 || count < 0)
                throw new ArgumentOutOfRangeException(index < 0 ? "index" : "count");

            if (chars.Length - index < count)
                throw new ArgumentOutOfRangeException("chars");

            // Handle an empty array
            if (chars.Length == 0)
                return 0;

            // Iterate each character, counting the bytes
            var byteCount = 0;
            foreach (var c in Common.Split(chars, index, count))
            {
                var value = table.GetByte(c);
                if (value == -1)
                {
                    throw new InvalidDataException($"Could not resolve character '{c}'");
                }

                byteCount++;
            }

            return byteCount;
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            throw new NotImplementedException();
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");

            if (index < 0 || count < 0)
                throw new ArgumentOutOfRangeException(index < 0 ? "index" : "count");

            if (bytes.Length - index < count)
                throw new ArgumentOutOfRangeException("bytes");

            throw new NotImplementedException();
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            throw new NotImplementedException();
        }

        public override int GetMaxByteCount(int charCount)
        {
            // TODO: from table, get longest constant and use it here
            //       for now, we assume that constants are no greater than 4 bytes
            return charCount * 4;
        }

        public override int GetMaxCharCount(int byteCount)
        {
            // the longest possible string length for a series of bytes
            return table.Characters.Max(x => x.Length) * byteCount;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the table use by this encoding.
        /// </summary>
        internal Table Table => table;

        #endregion
    }
}
