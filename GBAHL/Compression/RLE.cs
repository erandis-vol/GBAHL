using System;
using System.Collections.Generic;
using System.IO;

namespace GBAHL.Compression
{
    public static class RLE
    {
        public const byte Identifier = 0x30;

        public static byte[] Decompress(byte[] compressedBytes)
        {
            if (compressedBytes[0] != Identifier)
                throw new InvalidDataException();

            var length = (compressedBytes[1] | (compressedBytes[2] << 8) | (compressedBytes[3] << 16));
            var result = new byte[length];

            var i = 4;
            var j = 0;

            while (j < length)
            {
                var flag = compressedBytes[i++];

                var n = flag & 0x7F;
                var isCompressed = (flag & 0x80) == 0x80;

                if (isCompressed)
                {
                    for (int k = 0; k < n - 3; k++)
                    {
                        result[j++] = compressedBytes[i];
                    }

                    i++;
                }
                else
                {
                    for (int k = 0; k < n - 1; k++)
                    {
                        result[j++] = compressedBytes[i++];
                    }
                }
            }

            return result;
        }

        public static byte[] Compress(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
