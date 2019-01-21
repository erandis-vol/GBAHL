using System;
using System.Collections.Generic;
using System.IO;

namespace GBAHL.Compression
{
    public static class LZ
    {
        public const byte Identifier = 0x10;

        public static byte[] Decompress(byte[] bytes)
        {
            if (bytes[0] != Identifier)
                throw new InvalidDataException();

            var length = bytes[1] | (bytes[2] << 8) | (bytes[3] << 16);
            var result = new byte[length];

            var i = 4;
            var j = 0;

            while (j < length)
            {
                var flags = bytes[i++];

                for (int k = 7; k >= 0 && j < length; k--)
                {
                    var isCompressed = ((flags >> k) & 1) == 1;
                    if (isCompressed)
                    {
                        var data = (bytes[i++] << 8) | bytes[i++];

                        var n = (data >> 12) + 3;
                        var disp = j - (data & 0xFFF) - 1;

                        while (n-- > 0 && j < length)
                        {
                            result[j++] = result[disp++];
                        }
                    }
                    else
                    {
                        result[j++] = bytes[i++];
                    }
                }
            }

            return result;
        }

        // Compression originally by link12552

        public static byte[] Compress(byte[] bytes)
        {
            List<byte> compressed = new List<byte>();
            List<byte> preCompressed = new List<byte>();
            byte flags = 0;
            byte shortPos = 2;
            int actualPos = 2;
            int match = -1;

            int bestLength = 0;

            // Adds the LZ header to the bytes
            compressed.Add(Identifier);
            compressed.Add((byte)bytes.Length);
            compressed.Add((byte)(bytes.Length >> 8));
            compressed.Add((byte)(bytes.Length >> 16));

            // LZ Compression requires SOME starting data, so we provide the first 2 bytes
            preCompressed.Add(bytes[0]);
            preCompressed.Add(bytes[1]);

            // Compress everything
            while (actualPos < bytes.Length)
            {
                //If we've compressed 8 of 8 bytes
                if (shortPos == 8)
                {
                    // Add the Watch Mask
                    // Add the 8 steps in PreBytes
                    compressed.Add(flags);
                    compressed.AddRange(preCompressed);

                    flags = 0;
                    preCompressed.Clear();

                    // Back to 0 of 8 compressed bytes
                    shortPos = 0;
                }
                else
                {
                    // If we are approaching the end
                    if (actualPos + 1 < bytes.Length)
                    {
                        match = Search(
                            bytes,
                            actualPos,
                            Math.Min(4096, actualPos),
                            out bestLength
                        );
                    }
                    else
                    {
                        match = -1;
                    }

                    // If we have NOT found a match in the compression lookup
                    if (match == -1)
                    {
                        // Add the byte
                        preCompressed.Add(bytes[actualPos]);
                        // Add a 0 to the mask
                        //flags = BitConverter.GetBytes((int)flags << 1)[0];
                        flags <<= 1;

                        actualPos++;
                    }
                    else
                    {
                        // How many bytes match
                        int length = -1;

                        int start = match;
                        if (bestLength == -1)
                        {
                            // Old look-up technique
                            start = match;

                            bool isCompatible = true;
                            while (isCompatible == true && length < 18 && length + actualPos < bytes.Length - 1)
                            {
                                length++;
                                if (bytes[actualPos + length] != bytes[actualPos - start + length])
                                {
                                    isCompatible = false;
                                }
                            }
                        }
                        else
                        {
                            // New lookup (Perfect Compression!)
                            length = bestLength;
                        }

                        // Add the rel-compression pointer (P) and length of bytes to copy (L)
                        // Format: L P P P
                        //byte[] b = BitConverter.GetBytes(((length - 3) << 12) + (start - 1));
                        //b = new byte[] { b[1], b[0] };
                        //preCompressed.AddRange(b);
                        ushort ptr = (ushort)(((length - 3) << 12) + (start - 1));
                        preCompressed.Add((byte)(ptr >> 8));
                        preCompressed.Add((byte)ptr);

                        // Move to the next position
                        actualPos += length;

                        // Add a 1 to the bit Mask
                        //flags = BitConverter.GetBytes((flags << 1) + 1)[0];
                        flags <<= 1;
                        flags |= 1;
                    }

                    // We've just compressed 1 more 8
                    shortPos++;
                }
            }

            // Finnish off the compression
            if (shortPos != 0)
            {
                //Tyeing up any left-over data compression
                //flags = BitConverter.GetBytes((int)flags << (8 - shortPos))[0];
                flags <<= (8 - shortPos);

                compressed.Add(flags);
                compressed.AddRange(preCompressed);
            }

            // Return the Compressed bytes as an array!
            return compressed.ToArray();
        }

        private static int Search(byte[] bytes, int index, int length, out int match)
        {
            int pos = 2;
            match = 0;
            int found = -1;

            if (index + 2 < bytes.Length)
            {
                while (pos < length + 1 && match != 18)
                {
                    if (bytes[index - pos] == bytes[index] && bytes[index - pos + 1] == bytes[index + 1])
                    {

                        if (index > 2)
                        {
                            if (bytes[index - pos + 2] == bytes[index + 2])
                            {
                                int _match = 2;
                                bool isCompatible = true;
                                while (isCompatible == true && _match < 18 && _match + index < bytes.Length - 1)
                                {
                                    _match++;
                                    if (bytes[index + _match] != bytes[index - pos + _match])
                                    {
                                        isCompatible = false;
                                    }
                                }
                                if (_match > match)
                                {
                                    match = _match;
                                    found = pos;
                                }

                            }
                            pos++;
                        }
                        else
                        {
                            found = pos;
                            match = -1;
                            pos++;
                        }
                    }
                    else
                    {
                        pos++;
                    }
                }

                return found;
            }
            else
            {
                return -1;
            }
        }
    }
}
