using System;

namespace GBAHL.Drawing
{
    /// <summary>
    /// Represents a 15-bit BGR555 color.
    /// </summary>
    public struct Color2 : IEquatable<Color2>
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
        /// Tests whether two specified <see cref="Color2"/> values are equivalent.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static bool operator ==(Color2 c1, Color2 c2)
        {
            return c1.Equals(c2);
        }

        /// <summary>
        /// Tests whether two specified <see cref="Color2"/> values are not equivalent.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static bool operator !=(Color2 c1, Color2 c2)
        {
            return !(c1 == c2);
        }

        /// <summary>
        /// Determines whether this <see cref="Color2"/> is equivalent to the specified <see cref="Color2"/> value.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Color2 other)
        {
            return R == other.R
                && G == other.G
                && B == other.B;
        }

        /// <summary>
        /// Determines whether this <see cref="Color2"/> is equivalent to the specified object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (obj is Color2) && Equals((Color2)obj);
        }

        /// <summary>
        /// Returns the hash code for this <see cref="Color2"/> value.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return unchecked(R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode());
        }

        /// <summary>
        /// Returns a string representation of this <see cref="Color2"/> value.
        /// </summary>
        /// <returns>A string representation of this <see cref="Color2"/> value.</returns>
        public override string ToString()
        {
            return $"{{R={R}, G={G}, B={B}}}";
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
