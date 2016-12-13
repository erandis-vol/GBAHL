using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GBAHL.Drawing
{
    public class Palette : IEnumerable<Color>
    {
        private Color[] colors;

        public Palette(int size)
        {
            if (size != 16 && size != 256)
                throw new ArgumentOutOfRangeException("size");

            colors = new Color[size];
        }

        public Palette(Color[] colors)
        {
            if (colors.Length != 16 && colors.Length != 256)
                throw new ArgumentException();

            this.colors = colors;
        }

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

        public IEnumerator<Color> GetEnumerator()
        {
            return ((IEnumerable<Color>)colors).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Color>)colors).GetEnumerator();
        }

        public Color[] Colors
        {
            get { return colors; }
        }

        public int Length
        {
            get { return colors.Length; }
        }

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
