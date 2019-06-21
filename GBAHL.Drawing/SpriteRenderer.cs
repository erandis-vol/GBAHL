using System;

namespace GBAHL.Drawing
{
    public class SpriteRenderer : ISpriteRenderer<DirectBitmap>
    {
        public DirectBitmap Draw(Sprite sprite) => Draw(sprite, 0, 0);

        public DirectBitmap Draw(Sprite sprite, int x, int y)
        {
            var image = new DirectBitmap(sprite.Width * 8, sprite.Height * 8);
            Draw(image, sprite, x, y);
            return image;
        }

        public void Draw(DirectBitmap image, Sprite sprite, int x, int y)
        {
            if (sprite == null)
                throw new ArgumentNullException(nameof(sprite));

            Tileset tileset = sprite.Tileset;
            Palette palette = sprite.Palette;

            if (tileset.Length == 0)
                throw new ArgumentException("Sprite contains no tiles.", nameof(sprite));

            for (int i = 0; i < tileset.Length; i++)
            {
                DrawTile(image, ref tileset[i], palette, i % sprite.Width * 8, i / sprite.Width * 8);
            }

            if (sprite.HasExtraTiles)
            {
                Tile empty = new Tile();

                for (int i = tileset.Length; i < sprite.Width * sprite.Height; i++)
                {
                    DrawTile(null, ref empty, palette, x + i % sprite.Width * 8, y + i / sprite.Width * 8);
                }
            }
        }

        public void DrawTile(DirectBitmap image, ref Tile tile, Palette palette, int x, int y)
        {
            for (int j = 0; j < 8; j++)
            {
                for (int k = 0; k < 8; k++)
                {
                    image.SetPixel(x + k, y + j, palette[tile[k, j]].ToColor());
                }
            }
        }
    }
}
