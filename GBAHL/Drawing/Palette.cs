using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GBAHL.Drawing
{
    /// <summary>
    /// Represents the format of a palette file.
    /// </summary>
    public enum PaletteFormat
    {
        /// <summary>
        /// The default palette format.
        /// </summary>
        Default,
        /// <summary>
        /// Photoshop palette format.
        /// </summary>
        PAL,
        /// <summary>
        /// Adobe Color Table palette format.
        /// </summary>
        ACT
    }

    /// <summary>
    /// Represents a collection of colors.
    /// </summary>
    public class Palette : IEnumerable<Color>
    {
        private Color[] colors;

        /// <summary>
        /// Initializes a new instance of the <see cref="Palette"/> class with the specified length.
        /// </summary>
        /// <param name="color">The color to repeat.</param>
        /// <param name="length">The number of colors.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is less than zero.</exception>
        public Palette(Color color, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            colors = new Color[length];
            for (int i = 0; i < length; i++)
                colors[i] = color;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Palette"/> class
        /// for the specified bit depth.
        /// </summary>
        /// <param name="color">The color to repeat.</param>
        /// <param name="bitDepth">The bit depth.</param>
        public Palette(Color color, BitDepth bitDepth)
            : this(color, (int)bitDepth)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Palette"/> class from given array of <see cref="Color"/> values.
        /// </summary>
        /// <param name="colors">An array of color values.</param>
        /// <exception cref="ArgumentNullException"><paramref name="colors"/> is null.</exception>
        public Palette(Color[] colors)
        {
            this.colors = colors ?? throw new ArgumentNullException("colors");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Palette"/> class from the specified file.
        /// </summary>
        /// <param name="filename">The file name and path of the palette.</param>
        /// <param name="format">The expected format of the file.</param>
        /// <exception cref="FileNotFoundException">the file could not be found.</exception>
        /// <exception cref="InvalidDataException">the file is not formatted as expected.</exception>
        /// <exception cref="NotSupportedException"><paramref name="format"/> is not supported for loading.</exception>
        public Palette(string filename, PaletteFormat format = PaletteFormat.Default)
            : this(new FileInfo(filename), format)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Palette"/> class from the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="format">The expected format of the file.</param>
        /// <exception cref="FileNotFoundException">the file could not be found.</exception>
        /// <exception cref="InvalidDataException">the file is not formatted as expected.</exception>
        /// <exception cref="NotSupportedException"><paramref name="format"/> is not supported for loading.</exception>
        public Palette(FileInfo file, PaletteFormat format = PaletteFormat.Default)
        {
            switch (format)
            {
                case PaletteFormat.Default:
                    using (var br = new BinaryReader(file.OpenRead()))
                    {
                        var p = br.ReadByte();
                        var l = br.ReadByte();
                        var t = br.ReadByte();
                        var _ = br.ReadByte();

                        if (p != 'P' ||
                            l != 'L' ||
                            t != 'T' ||
                            _ != ' ')
                            throw new InvalidDataException("This is not a palette file.");

                        var length = br.ReadInt32();

                        colors = new Color[length];
                        for (int i = 0; i < length; i++)
                            colors[i] = Color.FromArgb(br.ReadInt32());                        
                    }
                    break;

                case PaletteFormat.PAL:
                    using (var sr = file.OpenText())
                    {
                        if (sr.ReadLine() != "JASC-PAL")
                            throw new InvalidDataException("This is not a PAL file.");
                        if (sr.ReadLine() != "0100")
                            throw new InvalidDataException("Unsupported PAL version.");

                        var length = 0;
                        if (!int.TryParse(sr.ReadLine(), out length))
                            throw new InvalidDataException("Invalid palette length.");

                        colors = new Color[length];
                        for (int i = 0; i < length; i++)
                        {
                            try
                            {
                                var color = sr.ReadLine().Split(' ');

                                var r = byte.Parse(color[0]);
                                var g = byte.Parse(color[1]);
                                var b = byte.Parse(color[2]);

                                colors[i] = Color.FromArgb(r, g, b);
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidDataException($"Invalid color on line {i + 4}.", ex);
                            }
                        }
                    }
                    break;

                case PaletteFormat.ACT:
                    using (var br = new BinaryReader(file.OpenRead()))
                    {
                        // NOTE: it is difficult to verify the format of an ACT palette
                        try
                        {
                            colors = new Color[256];
                            for (int i = 0; i < 256; i++)
                            {
                                var r = br.ReadByte();
                                var g = br.ReadByte();
                                var b = br.ReadByte();

                                colors[i] = Color.FromArgb(r, g, b);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidDataException("Invalid color table.", ex);
                        }
                    }
                    break;

                default:
                    throw new NotSupportedException($"Palette format {format} is not supported for loading.");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Palette"/> class from an indexed <see cref="Image"/>.
        /// </summary>
        /// <param name="image">The source image.</param>
        /// <exception cref="FormatException"><paramref name="image"/> is not indexed.</exception>
        public Palette(Image image)
        {
            // Initialize colors based on image bit depth
            switch (image.PixelFormat)
            {
                case PixelFormat.Format1bppIndexed:
                    colors = new Color[1 << 1];
                    break;

                case PixelFormat.Format4bppIndexed:
                    colors = new Color[1 << 4];
                    break;

                case PixelFormat.Format8bppIndexed:
                    colors = new Color[1 << 8];
                    break;

                default:
                    throw new FormatException();
            }

            // Copy colors from image palette
            image.Palette.Entries.CopyTo(colors, 0);
        }

        /// <summary>
        /// Gets or sets the specified color.
        /// </summary>
        /// <param name="index">The color's index.</param>
        /// <returns>A color.</returns>
        public Color this[int index]
        {
            get => colors[index];
            set => colors[index] = value;
        }

        /// <summary>
        /// Saves this <see cref="Palette"/> to the specified file.
        /// </summary>
        /// <param name="filename">The file name and path to save as.</param>
        /// <param name="format">the format to save the palette as.</param>
        /// <exception cref="NotSupportedException"><paramref name="format"/> is not supported for saving.</exception>
        public void Save(string filename, PaletteFormat format = PaletteFormat.Default)
        {
            Save(new FileInfo(filename), format);
        }


        /// <summary>
        /// Saves this <see cref="Palette"/> to the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="format">the format to save the palette as.</param>
        /// <exception cref="NotSupportedException"><paramref name="format"/> is not supported for saving.</exception>
        public void Save(FileInfo file, PaletteFormat format = PaletteFormat.Default)
        {
            switch (format)
            {
                case PaletteFormat.Default:
                    using (var bw = new BinaryWriter(file.Create()))
                    {
                        bw.Write((byte)'P');
                        bw.Write((byte)'L');
                        bw.Write((byte)'T');
                        bw.Write((byte)' ');

                        bw.Write(colors.Length);
                        foreach (var color in colors)
                            bw.Write(color.ToArgb());
                    }
                    break;

                case PaletteFormat.PAL:
                    using (var sw = file.CreateText())
                    {
                        sw.WriteLine("JASC-PAL");
                        sw.WriteLine("0100");
                        sw.WriteLine(colors.Length);

                        for (int i = 0; i < colors.Length; i++)
                        {
                            sw.WriteLine("{0} {1} {2}", colors[i].R, colors[i].G, colors[i].B);
                        }
                    }
                    break;

                case PaletteFormat.ACT:
                    // NOTE: color tables only support 256 color palettes
                    if (colors.Length != 256)
                        goto default;

                    using (var bw = new BinaryWriter(file.Create()))
                    {
                        foreach (var color in colors)
                        {
                            bw.Write(color.R);
                            bw.Write(color.G);
                            bw.Write(color.B);
                        }
                    }
                    break;

                default:
                    throw new NotSupportedException($"Palette format {format} is not supported for saving.");
            }
        }

        /// <summary>
        /// Returns an iterator that iterates through the <see cref="Palette"/>.
        /// </summary>
        /// <returns>A interator.</returns>
        public IEnumerator<Color> GetEnumerator() =>
             ((IEnumerable<Color>)colors).GetEnumerator();

        /// <summary>
        /// Returns an iterator that iterates through the <see cref="Palette"/>.
        /// </summary>
        /// <returns>An interator.</returns>
        IEnumerator IEnumerable.GetEnumerator() =>
            colors.GetEnumerator();

        /// <summary>
        /// Gets the number of colors in this <see cref="Palette"/>.
        /// </summary>
        public int Length => colors.Length;
    }
}
