using System;
using System.Drawing;

namespace GBAHL.Drawing.GDI
{
    /// <summary>
    /// Provides methods for colors.
    /// </summary>
    public static class ColorHelper
    {
        /// <summary>
        /// Converts this <see cref="Color2"/> instance to the equivalent <see cref="Color"/> instance.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ToColor(this Color2 color)
        {
            return Color.FromArgb(color.R << 3, color.G << 3, color.B << 3);
        }

        /// <summary>
        /// Converts this <see cref="Color"/> instance to the equivalent <see cref="Color2"/> instance.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color2 ToColor2(this Color color)
        {
            return Color2.FromArgb(color.R, color.G, color.B);
        }
    }
}
