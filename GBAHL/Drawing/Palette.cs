using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GBAHL.Drawing
{
    /// <summary>
    /// Represents a collection of 16 or 256 colors.
    /// </summary>
    public class Palette : IEnumerable<Color>
    {
        private Color[] colors;

        /// <summary>
        /// Creates a new <see cref="Palette"/> of the given size.
        /// </summary>
        /// <param name="size">The number of colors.</param>
        /// <exception cref="ArgumentOutOfRangeException">if the size is no 16 or 256 colors.</exception>
        public Palette(int size)
        {
            if (size != 16 && size != 256)
                throw new ArgumentOutOfRangeException("size");

            colors = new Color[size];
        }

        /// <summary>
        /// Creates a new <see cref="Palette"/> from given <see cref="Color"/> array.
        /// </summary>
        /// <param name="colors"></param>
        /// <exception cref="ArgumentException">if there are not 16 or 256 colors in the array.</exception>
        public Palette(Color[] colors)
        {
            if (colors.Length != 16 && colors.Length != 256)
                throw new ArgumentException();

            this.colors = colors;
        }

        /// <summary>
        /// Creates a new <see cref="Palette"/> from the specified palette file.
        /// </summary>
        /// <param name="filename">The palette file.</param>
        /// <exception cref="Exception">if the file has an unsupported format (expects standard JASC palette).</exception>
        /// <exception cref="Exception">the palette does not contain 16 or 256 colors.</exception>
        public Palette(string filename)
        {
            using (var sr = File.OpenText(filename))
            {
                if (sr.ReadLine() != "JASC-PAL")
                    throw new Exception("This is not a palette file!");
                if (sr.ReadLine() != "0100")
                    throw new Exception("Unsupported palette file format!");

                int length = int.Parse(sr.ReadLine());
                if (length != 16 && length != 256)
                    throw new Exception($"Palette contains {length} colors!\nMust be either 16 or 256.");

                colors = new Color[length];
                for (int i = 0; i < length; i++)
                {
                    // parse a color entry
                    // does not check format in order to let exceptions be thrown
                    string[] color = sr.ReadLine().Split(' ');

                    byte r = byte.Parse(color[0]);
                    byte g = byte.Parse(color[1]);
                    byte b = byte.Parse(color[2]);

                    colors[i] = Color.FromArgb(r, g, b);
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="Palette"/> from an indexed <see cref="Image"/>.
        /// </summary>
        /// <param name="image">The source image.</param>
        public Palette(Image image)
        {
            // Copy a palette from a source image
            if (image.PixelFormat == PixelFormat.Format4bppIndexed)
            {
                colors = new Color[16];
                image.Palette.Entries.CopyTo(colors, 0);
            }
            else if (image.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                colors = new Color[256];
                image.Palette.Entries.CopyTo(colors, 0);
            }
            else throw new Exception("Image is not indexed!");
        }

        /// <summary>
        /// Gets or sets the given <see cref="Color"/> in this <see cref="Palette"/>.
        /// </summary>
        /// <param name="index">The color's index.</param>
        /// <returns>A color.</returns>
        public Color this[int index]
        {
            get
            {
                if (index < 0 || index >= colors.Length)
                    throw new ArgumentOutOfRangeException("index");
                
                return colors[index];
            }
            set
            {
                if (index < 0 || index >= colors.Length)
                    throw new ArgumentOutOfRangeException("index");

                colors[index] = value;
            }
        }

        /// <summary>
        /// Saves this <see cref="Palette"/> to the given file.
        /// </summary>
        /// <param name="filename">The name of the file.</param>
        public void Save(string filename)
        {
            using (var sw = File.CreateText(filename))
            {
                sw.WriteLine("JASC-PAL");
                sw.WriteLine("0100");
                sw.WriteLine(colors.Length);

                for (int i = 0; i < colors.Length; i++)
                    sw.WriteLine("{0} {1} {2}", colors[i].R, colors[i].G, colors[i].B);
            }
        }

        /// <summary>
        /// Returns an iterator that iterates through the <see cref="Palette"/>.
        /// </summary>
        /// <returns>A interator.</returns>
        public IEnumerator<Color> GetEnumerator()
        {
            return ((IEnumerable<Color>)colors).GetEnumerator();
        }

        /// <summary>
        /// Returns an iterator that iterates through the <see cref="Palette"/>.
        /// </summary>
        /// <returns>An interator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Color>)colors).GetEnumerator();
        }

        /// <summary>
        /// Gets the colors of this <see cref="Palette"/>.
        /// </summary>
        public Color[] Colors
        {
            get { return colors; }
        }

        /// <summary>
        /// Gets the number of colors in this <see cref="Palette"/>.
        /// </summary>
        public int Length
        {
            get { return colors.Length; }
        }

        /// <summary>
        /// A 16-color grayscale palette going from black to white.
        /// </summary>
        public static Palette Grayscale16 = new Palette(new[]
        {
            Color.FromArgb(8, 8, 8),
            Color.FromArgb(24, 24, 24),
            Color.FromArgb(40, 40, 40),
            Color.FromArgb(56, 56, 56),
            Color.FromArgb(72, 72, 72),
            Color.FromArgb(88, 88, 88),
            Color.FromArgb(104, 104, 104),
            Color.FromArgb(120, 120, 120),
            Color.FromArgb(136, 136, 136),
            Color.FromArgb(152, 152, 152),
            Color.FromArgb(168, 168, 168),
            Color.FromArgb(184, 184, 184),
            Color.FromArgb(200, 200, 200),
            Color.FromArgb(216, 216, 216),
            Color.FromArgb(232, 232, 232),
            Color.FromArgb(248, 248, 248),
        });

        /// <summary>
        /// A 16-color grayscale palette going from white to black.
        /// </summary>
        public static Palette Grayscale16Reversed = new Palette(new[]
        {
            Color.FromArgb(248, 248, 248),
            Color.FromArgb(232, 232, 232),
            Color.FromArgb(216, 216, 216),
            Color.FromArgb(200, 200, 200),
            Color.FromArgb(184, 184, 184),
            Color.FromArgb(168, 168, 168),
            Color.FromArgb(152, 152, 152),
            Color.FromArgb(136, 136, 136),
            Color.FromArgb(120, 120, 120),
            Color.FromArgb(104, 104, 104),
            Color.FromArgb(88, 88, 88),
            Color.FromArgb(72, 72, 72),
            Color.FromArgb(56, 56, 56),
            Color.FromArgb(40, 40, 40),
            Color.FromArgb(24, 24, 24),
            Color.FromArgb(8, 8, 8),
        });
    }
}
