using System;

namespace GBAHL.Drawing
{
    /// <summary>
    /// Represents a 15-bit BGR555 color.
    /// </summary>
    public struct Bgr555 : IEquatable<Bgr555>
    {
        private const byte MaxComponentValue = (1 << 5) - 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bgr555"/> struct for the specified 15-bit BGR555 color.
        /// </summary>
        /// <param name="bgr"></param>
        public Bgr555(ushort bgr)
        {
            R = (byte)(bgr & 0x1F);
            G = (byte)((bgr >> 5) & 0x1F);
            B = (byte)((bgr >> 10) & 0x1F);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bgr555"/> struct with the specified color components.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="r"/>, <paramref name="g"/>, or <paramref name="b"/> is outside the range [0, 31].
        /// </exception>
        public Bgr555(byte r, byte g, byte b)
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
        /// Creates a <see cref="Bgr555"/> structure for the specified 8-bit color components.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <returns></returns>
        public static Bgr555 FromRgb(int r, int g, int b)
        {
            return new Bgr555(
                (byte)(r >> 3),
                (byte)(g >> 3),
                (byte)(b >> 3)
            );
        }

        /// <summary>
        /// Returns the 15-bit BGR555 value of this <see cref="Bgr555"/> value.
        /// </summary>
        /// <returns></returns>
        public ushort ToUInt16()
        {
            return (ushort)(R | (G << 5) | (B << 10));
        }

        /// <summary>
        /// Tests whether two specified <see cref="Bgr555"/> values are equivalent.
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public static bool operator ==(Bgr555 b1, Bgr555 b2)
        {
            return b1.Equals(b2);
        }

        /// <summary>
        /// Tests whether two specified <see cref="Bgr555"/> values are not equivalent.
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public static bool operator !=(Bgr555 b1, Bgr555 b2)
        {
            return !(b1 == b2);
        }

        public static implicit operator Bgr555(ushort value)
        {
            return new Bgr555(value);
        }

        public static implicit operator ushort(Bgr555 value)
        {
            return value.ToUInt16();
        }

        /// <summary>
        /// Determines whether this <see cref="Bgr555"/> is equivalent to the specified <see cref="Bgr555"/> value.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Bgr555 other)
        {
            return R == other.R
                && G == other.G
                && B == other.B;
        }

        /// <summary>
        /// Determines whether this <see cref="Bgr555"/> is equivalent to the specified object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (obj is Bgr555) && Equals((Bgr555)obj);
        }

        /// <summary>
        /// Returns the hash code for this <see cref="Bgr555"/> value.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return unchecked(R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode());
        }

        /// <summary>
        /// Returns a string representation of this <see cref="Bgr555"/> value.
        /// </summary>
        /// <returns>A string representation of this <see cref="Bgr555"/> value.</returns>
        public override string ToString()
        {
            return $"{{{R}, {G}, {B}}}";
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

        /// <summary>
        /// Gets a color that has a BGR value of #0000.
        /// </summary>
        public static Bgr555 Black => new Bgr555(0x00, 0x00, 0x00);

        /// <summary>
        /// Gets a color that has a BGR value of #FF7F.
        /// </summary>
        public static Bgr555 White => new Bgr555(0x1F, 0x1F, 0x1F);
    }
}
