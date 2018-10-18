using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace GBAHL.Drawing
{
    /// <summary>
    /// Specifies the number of bits used for a single pixel.
    /// </summary>
    public enum BitDepth_old
    {
        /// <summary>
        /// Specifies 1 bit per pixel.
        /// </summary>
        One = 1 << 1,
        /// <summary>
        /// Specifies 2 bits per pixel.
        /// </summary>
        Two = 1 << 2,
        /// <summary>
        /// Specifies 4 bits per pixel.
        /// </summary>
        Four = 1 << 4,
        /// <summary>
        /// Specifies 8 bits per pixel.
        /// </summary>
        Eight = 1 << 8,
    }

    public class Sprite_old
    {
        private Tile_old[] tiles;
        private BitDepth_old bitDepth;

        /// <summary>
        /// Creates a new <see cref="Sprite_old"/> of the given size (in tiles) and bit depth.
        /// </summary>
        /// <param name="width">The width in tiles.</param>
        /// <param name="height">The height in tiles.</param>
        /// <param name="bitDepth">The bit depth.</param>
        public Sprite_old(int width, int height, BitDepth_old bitDepth)
        {
            this.tiles = new Tile_old[width * height];
            this.bitDepth = bitDepth;
        }

        /// <summary>
        /// Creates a new <see cref="Sprite_old"/> for the given tiles and bit depth.
        /// </summary>
        /// <param name="tiles">The source tile data.</param>
        /// <param name="bitDepth">The bit depth.</param>
        public Sprite_old(Tile_old[] tiles, BitDepth_old bitDepth)
        {
            this.tiles = tiles;
            this.bitDepth = bitDepth;
        }

        /// <summary>
        /// Creates a new <see cref="Sprite_old"/> from an indexed bitmap.
        /// </summary>
        /// <param name="bmp"></param>
        public Sprite_old(Bitmap bmp)
        {
            // TODO: maybe help the user out and index their image for them
            if (bmp.PixelFormat != PixelFormat.Format4bppIndexed && bmp.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new Exception($"Source image is not indexed! {bmp.PixelFormat}");

            // ensure bitmap size
            if (bmp.Width % 8 != 0 || bmp.Height % 8 != 0)
                throw new Exception("Source image dimensions are not divisible by 8!");

            // lock the bits of the bitmap
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

            // create tiles
            int width = bmp.Width / 8;
            int height = bmp.Height / 8;

            tiles = new Tile_old[width * height];
            for (int i = 0; i < tiles.Length; i++)
                tiles[i] = new Tile_old();

            // copy pixel data
            if (bmp.PixelFormat == PixelFormat.Format4bppIndexed)
            {
                //palette = new Palette(16);
                bitDepth = BitDepth_old.Four;

                // copy the source data to a byte buffer
                byte[] buffer = new byte[bmp.Width * bmp.Height / 2];
                Marshal.Copy(bmpData.Scan0, buffer, 0, buffer.Length);
                
                // copy pixel data
                int x = 0;  // current x-position in tile
                int y = 0;  // current y-position in tile
                int a = 0;  // current x-position of tile
                int b = 0;  // current y-position of tile

                for (int i = 0; i < bmp.Width * bmp.Height / 2; i++)
                {
                    // 4BPP: two pixels per byte
                    tiles[a + b * width].SetPixel(x++, y, buffer[i] >> 4);  // left
                    tiles[a + b * width].SetPixel(x++, y, buffer[i] & 0xF); // right

                    // move forward positions
                    if (x >= 8)
                    {
                        x = 0;
                        a++;
                    }

                    if (a >= width)
                    {
                        a = 0;
                        y++;
                    }

                    if (y >= 8)
                    {
                        y = 0;
                        b++;
                    }
                }
            }
            else if (bmp.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                // above, modified for 8BPP (one pixel per byte)
                //palette = new Palette(256);
                bitDepth = BitDepth_old.Eight;

                byte[] buffer = new byte[bmp.Width * bmp.Height];
                Marshal.Copy(bmpData.Scan0, buffer, 0, buffer.Length);

                int x = 0;
                int y = 0;
                int a = 0;
                int b = 0;

                for (int i = 0; i < bmp.Width * bmp.Height; i++)
                {
                    tiles[a + b * width].SetPixel(x++, y, buffer[i]);

                    if (x >= 8)
                    {
                        x = 0;
                        a++;
                    }

                    if (a >= width)
                    {
                        a = 0;
                        y++;
                    }

                    if (y >= 8)
                    {
                        y = 0;
                        b++;
                    }
                }
            }

            // unlock the bitmap
            bmp.UnlockBits(bmpData);
        }

        /// <summary>
        /// Returns a new <see cref="Sprite_old"/> from raw 4 bpp (16 color) tile data.
        /// </summary>
        public static unsafe Sprite_old From4Bpp(byte[] buffer)
        {
            Tile_old[] tiles = new Tile_old[buffer.Length / 32];

            for (int i = 0; i < buffer.Length / 32; i++)
            {
                fixed (byte* ptr = &buffer[i * 32])
                {
                    Tile_old tile = new Tile_old();

                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 4; x++)
                        {
                            tile.SetPixel(x * 2, y, ptr[x + y * 4] & 0xF);
                            tile.SetPixel(x * 2 + 1, y, ptr[x + y * 4] >> 4);
                        }
                    }

                    tiles[i] = tile;
                }
            }

            return new Sprite_old(tiles, BitDepth_old.Four);
        }

        /// <summary>
        /// Creates a new <see cref="Sprite_old"/> from raw 8 bpp (256 color) tile data.
        /// </summary>
        public static unsafe Sprite_old From8Bpp(byte[] buffer)
        {
            Tile_old[] tiles = new Tile_old[buffer.Length / 64];

            for (int i = 0; i < buffer.Length / 64; i++)
            {
                fixed (byte* ptr = &buffer[i * 64])
                {
                    Tile_old tile = new Tile_old();

                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            tile.SetPixel(x, y, ptr[x + y * 8]);
                        }
                    }

                    tiles[i] = tile;
                }
            }

            return new Sprite_old(tiles, BitDepth_old.Eight);
        }

        /// <summary>
        /// Converts this <see cref="Sprite_old"/> into raw 4 bpp (16 color) tile data.
        /// </summary>
        /// <returns></returns>
        public unsafe byte[] To4Bpp()
        {
            byte[] buffer = new byte[tiles.Length * 32];
            for (int i = 0; i < tiles.Length; i++)
            {
                Tile_old tile = tiles[i];
                if (tile == null)
                    continue;

                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        buffer[(i * 32) + (x + y * 4)] = (byte)((tile.GetPixel(x * 2 + 1, y) << 4) | tile.GetPixel(x * 2, y));
                    }
                }
            }
            return buffer;
        }

        /// <summary>
        /// Converts this <see cref="Sprite_old"/> into raw 8 bpp (256 color) tile data.
        /// </summary>
        /// <returns></returns>
        public unsafe byte[] To8Bpp()
        {
            byte[] buffer = new byte[tiles.Length * 64];
            for (int i = 0; i < tiles.Length; i++)
            {
                Tile_old tile = tiles[i];
                if (tile == null)
                    continue;

                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        buffer[(i * 64) + (x + y * 8)] = tile.GetPixel(x, y);
                    }
                }
            }
            return buffer;
        }

        /// <summary>
        /// Converts this <see cref="Sprite_old"/> into an <see cref="Image"/> using the given <see cref="Palette"/>.
        /// </summary>
        /// <param name="size">The size in tiles.</param>
        /// <param name="palette">The <see cref="Palette"/>.</param>
        /// <param name="showColor0">If <c>false</c>, the first color in the palette is made transparent.</param>
        /// <returns></returns>
        public Image ToImage(Size size, Palette palette, bool showColor0 = true)
        {
            return ToImage(size.Width, size.Height, palette, showColor0);
        }

        /// <summary>
        /// Converts this <see cref="Sprite_old"/> into an <see cref="Image"/> using the given <see cref="Palette"/>.
        /// </summary>
        /// <param name="width">The width in tiles.</param>
        /// <param name="height">The height in tiles.</param>
        /// <param name="palette">The <see cref="Palette"/>.</param>
        /// <param name="showColor0">If <c>false</c>, the first color in the palette is made transparent.</param>
        /// <returns></returns>
        public Image ToImage(int width, int height, Palette palette, bool showColor0 = true)
        {
            Bitmap bmp = new Bitmap(width * Tile_old.Width, height * Tile_old.Height);

            // Draw tile data
            for (int tY = 0; tY < height; tY++)
            {
                for (int tX = 0; tX < width; tX++)
                {
                    Tile_old tile = tiles[tX + tY * width];
                    if (tile == null)
                        continue;

                    for (int y = 0; y < Tile_old.Width; y++)
                    {
                        for (int x = 0; x < Tile_old.Height; x++)
                        {
                            bmp.SetPixel(x + tX * Tile_old.Width, y + tY * Tile_old.Height, palette[tile.GetPixel(x, y)]);
                        }
                    }
                }
            }

            // Apply color 0 transparency
            if (!showColor0) {
                bmp.MakeTransparent(palette[0]);
            }

            return bmp;
        }

        /// <summary>
        /// Saves this <see cref="Sprite_old"/> as a bitmap.
        /// </summary>
        /// <param name="filename">A string that contains the name of the file to which to save this <see cref="Sprite_old"/>.</param>
        /// <param name="size">Size in tiles.</param>
        /// <param name="palette">A <see cref="Palette"/> to provide colors for the <see cref="Sprite_old"/>.</param>
        public void Save(string filename, Size size, Palette palette)
        {
            Save(filename, size.Width, size.Height, palette);
        }

        /// <summary>
        /// Saves this <see cref="Sprite_old"/> as a bitmap.
        /// </summary>
        /// <param name="filename">A string that contains the name of the file to which to save this <see cref="Sprite_old"/>.</param>
        /// <param name="w">Width in tiles.</param>
        /// <param name="h">Height in tiles.</param>
        /// <param name="palette">A <see cref="Palette"/> to provide colors for the <see cref="Sprite_old"/>.</param>
        public void Save(string filename, int w, int h, Palette palette)
        {
            int width = w * 8;  // width in pixels
            int height = h * 8; // height in pixels

            // custom bitmap writer
            // https://en.wikipedia.org/wiki/BMP_file_format

            // some calculations
            int rowSize = ((BitsPerPixel * width + 31) / 32) * 4;   // number of bytes per row
            int pixelSize = rowSize * height;                   // number of pixels in bytes
            int paddingSize = rowSize % 4;                      // number of extra bytes per row

            // create a buffer to hold the tiles
            byte[] buffer = new byte[width * height];

            // copy tiles to buffer
            for (int tY = 0; tY < h; tY++)
            {
                for (int tX = 0; tX < w; tX++)
                {
                    Tile_old tile = tiles[tX + tY * w];
                    if (tile == null)
                        continue;

                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            buffer[(x + tX * 8) + (y + tY * 8) * width] = tile.GetPixel(x, y);
                        }
                    }
                }
            }

            // write buffer to file
            using (var bw = new BinaryWriter(File.Create(filename)))
            {
                // TODO: Indexed images may be compressed using RLE or Huffman
                if (BitDepth == BitDepth_old.Four)
                {
                    // Bitmap file header
                    bw.Write((ushort)0x4D42);               // 'BM'
                    bw.Write(pixelSize + (16 * 4) + 54);    // filesize = header + color table + pixel data
                    bw.Write(0x293A);                       // embed a friendly message
                    bw.Write(54 + (16 * 4));                // offset of pixel data

                    // BITMAPINFOHEADER
                    bw.Write(40);               // header size = 40 bytes
                    bw.Write(width);            // width in pixels
                    bw.Write(height);           // height in pixels
                    bw.Write((ushort)1);        // 1 color plane
                    bw.Write((ushort)4);        // 8 bpp
                    bw.Write(0);                // no compression
                    bw.Write(pixelSize);        // size of raw data + padding
                    bw.Write(2835);             // print resoltion of image (~72 dpi)
                    bw.Write(2835);             //
                    bw.Write(16);               // color table size, 16 because MUST be 2^n
                    bw.Write(0);                // all colors are important

                    // color table
                    for (int i = 0; i < 16; i++)
                    {
                        var color = (i < palette.Length ? palette[i] : Color.Black);

                        bw.Write(color.B);
                        bw.Write(color.G);
                        bw.Write(color.R);
                        bw.Write(byte.MaxValue);
                    }

                    // pixel data
                    for (int y = height - 1; y >= 0; y--)
                    {
                        // copy colors for this row
                        for (int x = 0; x < width; x += 2)
                        {
                            //var left = buffer[x + y * width];
                            //var right = buffer[x + y * width + 1];
                            //var pixel = (left << 4) | right;
                            bw.Write((byte)((buffer[x + y * width] << 4) | buffer[x + y * width + 1]));
                        }

                        // include the last pixel in odd number widths
                        if (width % 2 != 0)
                        {
                            bw.Write((byte)(buffer[(width - 1) + y * width] << 4));
                        }

                        // pad end of row with 0's
                        for (int x = 0; x < paddingSize; x++)
                        {
                            bw.Write(byte.MinValue);
                        }
                    }
                }
                else //if (bitDepth == BitDepth.Eight)
                {
                    // Bitmap file header
                    bw.Write((ushort)0x4D42);               // 'BM'
                    bw.Write(pixelSize + (256 * 4) + 54);   // filesize = header + color table + pixel data
                    bw.Write(0x293A);                       // embed a friendly message
                    bw.Write(54 + (256 * 4));               // offset of pixel data

                    // BITMAPINFOHEADER
                    bw.Write(40);               // header size = 40 bytes
                    bw.Write(width);            // width in pixels
                    bw.Write(height);           // height in pixels
                    bw.Write((ushort)1);        // 1 color plane
                    bw.Write((ushort)8);        // 8 bpp
                    bw.Write(0);                // no compression
                    bw.Write(pixelSize);        // size of raw data + padding
                    bw.Write(2835);             // print resoltion of image (~72 dpi)
                    bw.Write(2835);             //
                    bw.Write(256);              // color table size, 256 because MUST be 2^n
                    bw.Write(0);                // all colors are important

                    // color table
                    for (int i = 0; i < 256; i++)
                    {
                        var color = (i < palette.Length ? palette[i] : Color.Black);

                        bw.Write(color.B);
                        bw.Write(color.G);
                        bw.Write(color.R);
                        bw.Write(byte.MaxValue);
                    }

                    // pixel data
                    for (int y = height - 1; y >= 0; y--)
                    {
                        // copy colors for this row
                        for (int x = 0; x < width; x++)
                        {
                            bw.Write((byte)buffer[x + y * width]);
                        }

                        // pad end of row with 0's
                        for (int x = 0; x < paddingSize; x++)
                        {
                            bw.Write(byte.MinValue);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Saves this <see cref="Sprite_old"/> as raw data.
        /// </summary>
        /// <param name="filename">A string that contains the name of the file to which to save this <see cref="Sprite_old"/>.</param>
        public void SaveRaw(string filename)
        {
            // NOTE: copy of To8Bpp method, without palette check

            // create a buffer to hold the tiles
            byte[] buffer = new byte[tiles.Length * 64];

            // copy tiles to buffer
            for (int i = 0; i < tiles.Length; i++)
            {
                Tile_old tile = tiles[i];
                if (tile == null)
                    continue;

                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        buffer[i * 64 + x + y * 8] = tile.GetPixel(x, y);
                    }
                }
            }

            // write buffer to file
            File.WriteAllBytes(filename, buffer);
        }

        /// <summary>
        /// Changes the number of tiles in this <see cref="Sprite_old"/>.
        /// </summary>
        /// <param name="numberOfTiles">The new number of tiles.</param>
        public void Resize(int numberOfTiles)
        {
            Array.Resize(ref tiles, numberOfTiles);
        }

        /// <summary>
        /// Removes any null tiles from the end of the <see cref="Sprite_old"/>.
        /// </summary>
        public void TrimNullTiles()
        {
            int i = tiles.Length - 1;
            while (i > 0 && tiles[i] == null)
            {
                i--;
            }

            Array.Resize(ref tiles, i);
        }

        /// <summary>
        /// Returns an array of sizes for which this <see cref="Sprite_old"/> can be displayed cleanly.
        /// </summary>
        public Size[] ValidDimensions
        {
            get
            {
                if (tiles.Length == 0)
                    //return new[] { new Size(0, 0) };
                    return new Size[0];

                var result = new List<Size>();
                for (int width = 1; width <= tiles.Length; width++)
                {
                    if (tiles.Length % width == 0)
                        result.Add(new Size(width, tiles.Length / width));
                }
                return result.ToArray();
            }
        }

        /// <summary>
        /// lol
        /// </summary>
        public Tile_old[] Tiles
        {
            get { return tiles; }
        }

        /// <summary>
        /// Gets the number of tiles in the <see cref="Sprite_old"/>.
        /// </summary>
        public int Size
        {
            get { return tiles.Length; }
        }

        /// <summary>
        /// Gets the bit depth of this <see cref="Sprite_old"/>.
        /// </summary>
        public BitDepth_old BitDepth
        {
            get { return bitDepth; }
        }
        
        /// <summary>
        /// Gets the number of bits per pixel of this <see cref="Sprite_old"/>.
        /// </summary>
        public int BitsPerPixel
        {
            get { return (int)bitDepth; }
        }
    }
}
