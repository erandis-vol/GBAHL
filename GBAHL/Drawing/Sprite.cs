using System;

namespace GBAHL.Drawing
{
    public class Sprite
    {
        private int width;
        private int height;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class for the specified tileset and palette.
        /// </summary>
        /// <param name="tileset"></param>
        /// <param name="palette"></param>
        /// <param name="width"></param>
        public Sprite(Tileset tileset, Palette palette, int width = 1)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            Tileset = tileset ?? throw new ArgumentNullException(nameof(tileset));
            Palette = palette ?? throw new ArgumentNullException(nameof(palette));
            Width = width;
        }

        /// <summary>
        /// Returns an array containing the pixels of all tiles.
        /// </summary>
        /// <returns></returns>
        public Color2[] GetPixels()
        {
            var pixels = new Color2[Tileset.Length * 64];

            for (int i = 0; i < Tileset.Length; i++)
            {
                var tile = Tileset[i];
                for (int j = 0; j < 64; j++)
                    pixels[i * 64 + j] = Palette[tile[j]];
            }

            return pixels;
        }

        /// <summary>
        /// Gets or sets the tileset.
        /// </summary>
        public Tileset Tileset { get; set; }

        /// <summary>
        /// Gets or sets the palette.
        /// </summary>
        public Palette Palette { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public int Width
        {
            get => width;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(Width));

                if (width != value)
                {
                    width = value;

                    height = Tileset.Length / width;
                    if (Tileset.Length % width != 0)
                        height++;
                }
            }
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public int Height => height;

        /// <summary>
        /// Determines whether the sprite has tiles beyond the length of the tileset.
        /// </summary>
        public bool HasExtraTiles => width * height > Tileset.Length;
    }
}
