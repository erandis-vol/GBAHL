using OpenTK;
using OpenTK.Graphics;

namespace GBAHL.Drawing.OpenGL
{
    public static class ColorHelper
    {
        /// <summary>
        /// Converts this <see cref="Color2"/> instance to the equivalent <see cref="Color4"/> instance.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color4 ToColor4(this Color2 color)
        {
            return new Color4(
                (byte)(color.R << 3),
                (byte)(color.G << 3),
                (byte)(color.B << 3),
                255
            );
        }
    }
}
