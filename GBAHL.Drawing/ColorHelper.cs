using System;
using System.Drawing;

namespace GBAHL.Drawing
{
    /// <summary>
    /// Provides methods for colors.
    /// </summary>
    public static class ColorHelper
    {
        /// <summary>
        /// Converts this <see cref="Bgr555"/> instance to the equivalent <see cref="Color"/> instance.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ToColor(this Bgr555 color)
        {
            return Color.FromArgb(color.R << 3, color.G << 3, color.B << 3);
        }

        /// <summary>
        /// Converts this <see cref="Color"/> instance to the equivalent <see cref="Bgr555"/> instance.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Bgr555 ToBgr555(this Color color)
        {
            return Bgr555.FromArgb(color.R, color.G, color.B);
        }
    }
}
