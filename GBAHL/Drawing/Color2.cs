using System;

namespace GBAHL.Drawing
{
    /// <summary>
    /// Represents a 15-bit BGR555 color.
    /// </summary>
    public struct Color2
    {
        private const byte MaxComponentValue = (1 << 5) - 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="Color2"/> struct for the specified 15-bit BGR555 color.
        /// </summary>
        /// <param name="bgr"></param>
        public Color2(ushort bgr)
        {
            R = (byte)((bgr & 0x1F) << 3);
            G = (byte)((bgr >> 5 & 0x1F) << 3);
            B = (byte)((bgr >> 10 & 0x1F) << 3);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color2"/> struct with the specified color components.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="r"/>, <paramref name="g"/>, or <paramref name="b"/> is outside the range [0, 31].
        /// </exception>
        public Color2(byte r, byte g, byte b)
        {
            if (r > MaxComponentValue)
                throw new ArgumentOutOfRangeException(nameof(r));

            if (g > MaxComponentValue)
                throw new ArgumentOutOfRangeException(nameof(g));

            if (b > MaxComponentValue)
                throw new ArgumentOutOfRangeException(nameof(b));

            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// Creates a <see cref="Color2"/> structure for the specified 8-bit color components.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <returns></returns>
        public static Color2 FromArgb(int r, int g, int b)
        {
            return new Color2(
                (byte)(r >> 3),
                (byte)(g >> 3),
                (byte)(b >> 3)
            );
        }

        /// <summary>
        /// Returns the 15-bit BGR555 value of this <see cref="Color2"/> value.
        /// </summary>
        /// <returns></returns>
        public ushort ToBgr()
        {
            return (ushort)(R | (G << 5) | (B << 10));
        }

        /// <summary>
        /// Gets the red component.
        /// </summary>
        public byte R { get; }

        /// <summary>
        /// Gets the green component.
        /// </summary>
        public byte G { get; }

        /// <summary>
        /// Gets the blue component.
        /// </summary>
        public byte B { get; }
    }
}
