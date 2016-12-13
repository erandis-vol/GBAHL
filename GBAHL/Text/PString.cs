using System;
using System.Collections.Generic;
using System.Linq;

namespace GBAHL.Text
{
    // Represents an encoded string.
    public class PString
    {
        //public static Table.Encoding DefaultEncoding = Table.Encoding.English;

        string str;                 // string data
        int originalSize;           // original size of string in bytes
        Table.Encoding encoding;    // table encoding for the string

        public PString(string str, Table.Encoding encoding)
        {
            this.str = str;
            this.originalSize = 0;
        }

        public PString(string str, int originalSize, Table.Encoding encoding)
        {
            this.str = str;
            this.originalSize = originalSize;
        }

        public PString(byte[] buffer, Table.Encoding encoding)
        {
            str = Table.GetString(buffer, encoding);
            originalSize = buffer.Length;
        }

        /*public static implicit operator Text(string str)
        {
            return new Text(str, DefaultEncoding);
        }*/

        public static implicit operator string(PString t)
        {
            return t.str;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="string"/> value of this <see cref="PString"/>.
        /// </summary>
        public string Value
        {
            get { return str; }
            set { str = value; }
        }

        /// <summary>
        /// Gets the original size of this <see cref="PString"/> in bytes.
        /// </summary>
        public int OriginalSize
        {
            get { return originalSize; }
        }

        /// <summary>
        /// Gets or sets the <see cref="Table.Encoding"/> of this <see cref="PString"/>.
        /// </summary>
        public Table.Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        #endregion
    }
}
