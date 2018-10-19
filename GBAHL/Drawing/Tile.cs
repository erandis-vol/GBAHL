using System;

namespace GBAHL.Drawing
{
    public struct Tile
    {
        private byte[] pixels;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tile"/> struct by copying pixels from the specified array.
        /// </summary>
        /// <param name="pixels">The pixels to be copied.</param>
        /// <exception cref="ArgumentException"><paramref name="pixels"/> does not contain 64 elements.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="pixels"/> is null.</exception>
        public Tile(byte[] pixels)
        {
            if (pixels == null)
                throw new ArgumentNullException(nameof(pixels));

            if (pixels.Length != 64)
                throw new ArgumentException("Expected 64 pixel elements.", nameof(pixels));

            this.pixels = (byte[])pixels.Clone();
        }

        /// <summary>
        /// Gets or sets the specified pixel.
        /// </summary>
        /// <param name="index">The index of the pixel.</param>
        /// <returns></returns>
        public byte this[int index]
        {
            get
            {
                if (pixels == null)
                {
                    return byte.MinValue;
                }

                return pixels[index];
            }
            set
            {
                if (pixels == null)
                {
                    pixels = new byte[64];
                }

                pixels[index] = value;
            }
        }

        /// <summary>
        /// Gets or sets the specified pixel.
        /// </summary>
        /// <param name="x">The x-coordinate of the pixel.</param>
        /// <param name="y">The y-coordinate of the pixel.</param>
        /// <returns></returns>
        public byte this[int x, int y]
        {
            get => this[x + y * 8];
            set => this[x + y * 8] = value;
        }

        public bool Equals(ref Tile other, bool flipX, bool flipY)
        {
            if (flipX || flipY)
            {
                // TODO: Is the following faster?
                //fixed (byte* src = &pixels[0])
                //fixed (byte* dst = &other.pixels[0])
                //{
                //    for (int srcY = 0; srcY < 8; srcY++)
                //    {
                //        for (int srcX = 0; srcX < 8; srcX++)
                //        {
                //            var dstX = flipX ? (7 - srcX) : srcX;
                //            var dstY = flipY ? (7 - srcY) : srcY;
                //            if (src[srcX + srcY * 8] != dst[dstX + dstY * 8])
                //                return false;
                //        }
                //    }
                //}

                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        var otherX = flipX ? (7 - x) : x;
                        var otherY = flipY ? (7 - y) : y;

                        if (this[x, y] != other[otherX, otherY])
                        {
                            return false;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 64; i++)
                {
                    if (this[i] != other[i]) return false;
                }
            }

            return true;
        }

        public bool Equals(ref Tile other)
        {
            return Equals(ref other, false, false);
        }
    }
}
