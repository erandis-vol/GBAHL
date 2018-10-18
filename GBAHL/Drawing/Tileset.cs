using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBAHL.Drawing
{
    public class Tileset
    {
        public struct Tile
        {
            private byte[] pixels;

            /// <summary>
            /// Initializes a new instance of the <see cref="Tile"/> struct by copying pixels from the specified array.
            /// </summary>
            /// <param name="pixels">The pixels to be copied.</param>
            /// <exception cref="ArgumentException">
            /// <paramref name="pixels"/> does not contain <c>64</c> pixels.
            /// </exception>
            /// <exception cref="ArgumentNullException"><paramref name="pixels"/> is <c>null</c>.</exception>
            public Tile(byte[] pixels)
            {
                if (pixels == null)
                    throw new ArgumentNullException(nameof(pixels));

                if (pixels.Length != 64)
                    throw new ArgumentException("Invalid pixel data.", nameof(pixels));

                this.pixels = (byte[])pixels.Clone();
            }

            /// <summary>
            /// The pixel data.
            /// </summary>
            public byte[] Pixels => pixels ?? (pixels = new byte[64]);
        }
    }
}
