using System;
using System.IO;
using System.Text;

namespace GBAHL.IO
{
    public static class Read
    {
        public static int ReadInt24(this BinaryReader reader)
        {
            return reader.ReadByte()
                | (reader.ReadByte() << 8)
                | (reader.ReadByte() << 16);
        }

        public static int ReadPointer(this BinaryReader reader)
        {
            var pointer = reader.ReadInt32();
            if (pointer == 0)
            {
                return 0;
            }

            var bank = (pointer >> 24) & 0xFF;
            if (bank != 8 && bank != 9)
            {
                return -1;
            }

            return pointer & 0x1FFFFFF;
        }

        public static string ReadString(this BinaryReader reader, int length)
        {
            var bytes = reader.ReadBytes(length);
            if (bytes == null || bytes.Length == 0)
            {
                return string.Empty;
            }

            return Encoding.UTF8.GetString(bytes).Replace("\0", "");
        }

        public static byte[] ReadCompressedBytes(this BinaryReader reader)
        {
            // Read compression ID
            var isCompressed = reader.ReadByte() == 0x10;
            if (!isCompressed)
            {
                reader.BaseStream.Position -= 1;
                return new byte[0];
            }

            // Read decompressed length
            var length = reader.ReadInt24();
            var buffer = new byte[length];

            // Read compressed bytes
            var s = 0;
            var i = 0;
            var f = 0;

            while (s < length)
            {
                if (i == 0)
                {
                    f = reader.ReadByte();
                }

                if ((f & (0x80 >> i)) == 0)
                {
                    // Read byte
                    buffer[s++] = reader.ReadByte();
                }
                else
                {
                    // Copy block
                    var b = (reader.ReadByte() << 8) | reader.ReadByte();

                    var c = (b >> 12) + 3;
                    var d = s - (b & 0xFFF) - 1;

                    while (c-- > 0 && s < length)
                    {
                        buffer[s++] = buffer[d++];
                    }
                }

                i = ++i % 8;
            }

            return buffer;
        }
    }
}
