using System;

namespace GBAHL.Drawing
{
    /// <summary>
    /// Provides methods for drawing sprites.
    /// </summary>
    public static class SpriteRenderer
    {
        /// <summary>
        /// Returns a new <see cref="FastBitmap"/> representing the sprite.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="sprite"/> is null.</exception>
        public static FastBitmap Draw(Sprite sprite)
        {
            if (sprite == null)
                throw new ArgumentNullException(nameof(sprite));

            if (sprite.Tileset.Length == 0)
                throw new ArgumentException("Sprite is empty.");

            var fb = new FastBitmap(sprite.Width << 3, sprite.Height << 3);

            var tileset = sprite.Tileset;
            var palette = sprite.Palette;
            var columns = sprite.Width;

            for (int i = 0; i < tileset.Length; i++)
            {
                // Get the destination
                var x = i % columns;
                var y = i / columns;

                // Get the tile to draw
                ref var tile = ref tileset[i];

                // Draw the tile
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        fb.SetPixel(x * 8 + k, y * 8 + j, palette[tile[k, j]].ToColor());
                    }
                }
            }

            if (sprite.HasExtraTiles)
            {
                // TODO
            }

            return fb;
        }
    }
}
