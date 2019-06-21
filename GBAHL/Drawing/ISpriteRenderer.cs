namespace GBAHL.Drawing
{
    /// <summary>
    /// Provides a mechanism for rendering a sprite as an image.
    /// </summary>
    /// <typeparam name="TImage"></typeparam>
    public interface ISpriteRenderer<TImage>
    {
        /// <summary>
        /// Draws the sprite as an image.
        /// </summary>
        /// <param name="sprite">The sprite to be drawn.</param>
        /// <returns></returns>
        TImage Draw(Sprite sprite);
    }
}
