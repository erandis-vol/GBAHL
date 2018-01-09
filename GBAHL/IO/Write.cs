using System;
using System.IO;
using System.Text;

namespace GBAHL.IO
{
    public static class Write
    {
        public static void WritePointer(this BinaryWriter writer, int offset)
        {
            if (offset < 0 || offset > 0x1FFFFFF)
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            writer.Write(offset > 0 ? offset | 0x08000000 : 0);
        }

        public static void WriteString(this BinaryWriter writer, string str, int length)
        {
            var buffer = Encoding.UTF8.GetBytes(str);
            if (buffer.Length != length)
            {
                Array.Resize(ref buffer, length);
            }
            writer.Write(buffer);
        }

        public static void WriteCompressed(this BinaryWriter writer, byte[] bytes)
        {
            // todo: return compressed length?
            writer.Write(Compression.LZSS.Compress(bytes));
        }
    }
}
