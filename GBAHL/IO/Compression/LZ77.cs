using System;
using System.Collections.Generic;

namespace GBAHL.IO.Compression
{
    public static class LZSS
    {
        public static byte[] Decompress(byte[] buffer)
        {
            // 0x10 marks lz77 compressed data
            if (buffer[0] != 0x10)
                throw new Exception("This data is not compressed!");

            // 24-bit length decompressed
            int length = buffer[1] | (buffer[2] << 8) | (buffer[3] << 16);
            byte[] result = new byte[length];

            // decompress
            int i = 4;      // position in result buffer
            int size = 0;   // size of data decompressed so far
            int pos = 0;    // current flag position
            int flags = 0;  // current working flags (byte)
            while (size < length)
            {
                if (pos == 0) flags = buffer[i++];

                if ((flags & (0x80 >> pos)) == 0)
                {
                    // copy value to result buffer
                    result[size++] = buffer[i++];
                }
                else
                {
                    // copy block from result buffer
                    int block = (buffer[i++] << 8) | buffer[i++];

                    int bytes = (block >> 12) + 3;
                    int disp = size - (block & 0xFFF) - 1;

                    while (bytes-- > 0 && size < length)
                    {
                        result[size++] = result[disp++];
                    }
                }

                pos = ++pos % 8;
            }

            return result;
        }

        // compression originally by link12552

        public static byte[] Compress(byte[] Data)
        {
            byte[] header = BitConverter.GetBytes(Data.Length);
            List<byte> Bytes = new List<byte>();
            List<byte> PreBytes = new List<byte>();
            byte Watch = 0;
            byte ShortPosition = 2;
            int ActualPosition = 2;
            int match = -1;

            int BestLength = 0;

            // Adds the Lz77 header to the bytes 0x10 3 bytes size reversed
            Bytes.Add(0x10);
            Bytes.Add(header[0]);
            Bytes.Add(header[1]);
            Bytes.Add(header[2]);

            // Lz77 Compression requires SOME starting data, so we provide the first 2 bytes
            PreBytes.Add(Data[0]);
            PreBytes.Add(Data[1]);

            // Compress everything
            while (ActualPosition < Data.Length)
            {
                //If we've compressed 8 of 8 bytes
                if (ShortPosition == 8)
                {
                    // Add the Watch Mask
                    // Add the 8 steps in PreBytes
                    Bytes.Add(Watch);
                    Bytes.AddRange(PreBytes);

                    Watch = 0;
                    PreBytes.Clear();

                    // Back to 0 of 8 compressed bytes
                    ShortPosition = 0;
                }
                else
                {
                    // If we are approaching the end
                    if (ActualPosition + 1 < Data.Length)
                    {
                        match = Search(
                                    Data,
                                    ActualPosition,
                                    Math.Min(4096, ActualPosition), out BestLength);
                    }
                    else
                    {
                        match = -1;
                    }

                    // If we have NOT found a match in the compression lookup
                    if (match == -1)
                    {
                        // Add the byte
                        PreBytes.Add(Data[ActualPosition]);
                        // Add a 0 to the mask
                        Watch = BitConverter.GetBytes((int)Watch << 1)[0];

                        ActualPosition++;
                    }
                    else
                    {
                        // How many bytes match
                        int length = -1;

                        int start = match;
                        if (BestLength == -1)
                        {
                            // Old look-up technique
                            start = match;

                            bool Compatible = true;
                            while (Compatible == true && length < 18 && length + ActualPosition < Data.Length - 1)
                            {
                                length++;
                                if (Data[ActualPosition + length] != Data[ActualPosition - start + length])
                                {
                                    Compatible = false;
                                }
                            }
                        }
                        else
                        {
                            // New lookup (Perfect Compression!)
                            length = BestLength;
                        }

                        // Add the rel-compression pointer (P) and length of bytes to copy (L)
                        // Format: L P P P
                        byte[] b = BitConverter.GetBytes(((length - 3) << 12) + (start - 1));

                        b = new byte[] { b[1], b[0] };
                        PreBytes.AddRange(b);

                        // Move to the next position
                        ActualPosition += length;

                        // Add a 1 to the bit Mask
                        Watch = BitConverter.GetBytes((Watch << 1) + 1)[0];
                    }

                    // We've just compressed 1 more 8
                    ShortPosition++;
                }
            }

            // Finnish off the compression
            if (ShortPosition != 0)
            {
                //Tyeing up any left-over data compression
                Watch = BitConverter.GetBytes((int)Watch << (8 - ShortPosition))[0];

                Bytes.Add(Watch);
                Bytes.AddRange(PreBytes);
            }

            // Return the Compressed bytes as an array!
            return Bytes.ToArray();
        }

        private static int Search(byte[] Data, int Index, int Length, out int match)
        {
            int pos = 2;
            match = 0;
            int found = -1;

            if (Index + 2 < Data.Length)
            {
                while (pos < Length + 1 && match != 18)
                {
                    if (Data[Index - pos] == Data[Index] && Data[Index - pos + 1] == Data[Index + 1])
                    {

                        if (Index > 2)
                        {
                            if (Data[Index - pos + 2] == Data[Index + 2])
                            {
                                int _match = 2;
                                bool Compatible = true;
                                while (Compatible == true && _match < 18 && _match + Index < Data.Length - 1)
                                {
                                    _match++;
                                    if (Data[Index + _match] != Data[Index - pos + _match])
                                    {
                                        Compatible = false;
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
