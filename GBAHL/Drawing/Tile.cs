using System;

namespace GBAHL.Drawing
{
    /// <summary>
    /// Represents an 8x8 pixel tile.
    /// </summary>
    public class Tile
    {
        /// <summary>
        /// The width of all tiles in pixels.
        /// </summary>
        public const int Width = 8;
        /// <summary>
        /// The height of all tiles in pixels.
        /// </summary>
        public const int Height = 8;

        byte[] data = new byte[Width * Height];

        /// <summary>
        /// Initializes a blank <see cref="Tile"/>.
        /// </summary>
        public Tile()
        { }

        /// <summary>
        /// Initializes a new <see cref="Tile"/> with the specified pixel data.
        /// </summary>
        /// <param name="buffer">A byte array holding pixel data.</param>
        internal Tile(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            if (buffer.Length != Width * Height)
                throw new Exception();

            data = buffer;
        }


        /// <summary>
        /// Gets the pixel at the specified position.
        /// </summary>
        /// <param name="x">The x-position of the pixel.</param>
        /// <param name="y">The y-position of the pixel.</param>
        /// <returns>The pixel at position (x, y).</returns>
        /// <exception cref="ArgumentOutOfRangeException">x was less than 0 or larger than 8; y was less than 0 or larger than 8.</exception>
        public byte GetPixel(int x, int y)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException("x");

            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException("y");

            return data[x + y * Width];
        }

        /// <summary>
        /// Sets the pixel at the specified position.
        /// </summary>
        /// <param name="x">The x-position of the pixel.</param>
        /// <param name="y">The y-position of the pixel.</param>
        /// <param name="value">The value of the pixel.</param>
        /// <exception cref="ArgumentOutOfRangeException">x was less than 0 or larger than 8.</exception>
        /// <exception cref="ArgumentOutOfRangeException">y was less than 0 or larger than 8.</exception>
        public void SetPixel(int x, int y, byte value)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException("x");

            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException("y");

            data[x + y * Width] = value;
        }

        /// <summary>
        /// Sets the pixel at the specified position.
        /// </summary>
        /// <param name="x">The x-position of the pixel.</param>
        /// <param name="y">The y-position of the pixel.</param>
        /// <param name="value">The value of the pixel.</param>
        /// <exception cref="ArgumentOutOfRangeException">x was less than 0 or larger than 8.</exception>
        /// <exception cref="ArgumentOutOfRangeException">y was less than 0 or larger than 8.</exception>
        /// <exception cref="ArgumentOutOfRangeException">value was less than 0 or larger than 255.</exception>
        public void SetPixel(int x, int y, int value)
        {
            if (value < byte.MinValue || value > byte.MaxValue)
                throw new ArgumentOutOfRangeException("value");

            SetPixel(x, y, (byte)value);
        }
    }
}
