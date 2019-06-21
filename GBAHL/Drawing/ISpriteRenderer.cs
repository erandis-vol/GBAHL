namespace GBAHL.Drawing
{
    /// <summary>
    /// Provides a mechanism for rendering a sprite as an image.
    /// </summary>
    /// <typeparam name="TImage"></typeparam>
    public interface ISpriteRenderer<TImage>
    {
        /// <summary>
        /// Draws a sprite as an image.
        /// </summary>
        /// <param name="sprite">The sprite to be drawn.</param>
        /// <returns></returns>
        TImage Draw(Sprite sprite, int x, int y);

        /// <summary>
        /// Draws a sprite to the specified image.
        /// </summary>
        /// <param name="image">The image to be drawn to.</param>
        /// <param name="sprite">The sprite to be drawn.</param>
        /// <param name="x">The x-coordinate of the sprite.</param>
        /// <param name="y">The y-coordinate of the sprite.</param>
        void Draw(TImage image, Sprite sprite, int x, int y);

        /// <summary>
        /// Draws a tile to the specified image.
        /// </summary>
        /// <param name="image">The image to be drawn to.</param>
        /// <param name="tile">The tile to be drawn.</param>
        /// <param name="palette">The palette to use.</param>
        /// <param name="x">The x-coordinate of the tile.</param>
        /// <param name="y">The y-coordinate of the tile.</param>
        void DrawTile(TImage image, ref Tile tile, Palette palette, int x, int y);
    }
}
