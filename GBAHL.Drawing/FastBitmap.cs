using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace GBAHL.Drawing
{
    public class FastBitmap : IDisposable
    {
        private Bitmap bitmap;
        private int[] bits;
        private GCHandle handle;
        private int width, height;

        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="FastBitmap"/> class with the specified size.
        /// </summary>
        /// <param name="width">The width, in pixels, of the <see cref="FastBitmap"/>.</param>
        /// <param name="height">The height, in pixels, of the <see cref="FastBitmap"/>.</param>
        public FastBitmap(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.bits = new int[width * height];
            this.handle = GCHandle.Alloc(this.bits, GCHandleType.Pinned);
            this.bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, handle.AddrOfPinnedObject());
        }

        /// <summary>
        /// Creates a new <see cref="FastBitmap"/> from the specified <see cref="Image"/>.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        public static FastBitmap FromImage(Image image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            var fast = new FastBitmap(image.Width, image.Height);
            using (var g = Graphics.FromImage(fast.bitmap))
            {
                g.DrawImage(image, 0, 0, image.Width, image.Height);
            }
            return fast;
        }

        ~FastBitmap()
        {
            Dispose();
        }

        #region Methods

        /// <summary>
        /// Releases all resources used by this <see cref="FastBitmap"/>.
        /// </summary>
        public void Dispose()
        {
            if (!isDisposed)
            {
                bitmap.Dispose();
                bitmap = null;
                handle.Free();
                isDisposed = true;
            }
        }

        /// <summary>
        /// Returns the specified pixel.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Color GetPixel(int x, int y)
        {
            if (x < 0 || x >= width)
                throw new ArgumentOutOfRangeException(nameof(x));

            if (y < 0 || y >= height)
                throw new ArgumentOutOfRangeException(nameof(y));

            return Color.FromArgb(bits[x + y * width]);
        }

        /// <summary>
        /// Sets the specified pixel.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void SetPixel(int x, int y, Color color)
        {
            if (x < 0 || x >= width)
                throw new ArgumentOutOfRangeException(nameof(x));

            if (y < 0 || y >= height)
                throw new ArgumentOutOfRangeException(nameof(y));

            bits[x + y * width] = color.ToArgb();
        }

        public static implicit operator Bitmap(FastBitmap fb) => fb.bitmap;

        public static implicit operator Image(FastBitmap fb) => fb.bitmap;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the bits of this image.
        /// </summary>
        public int[] Bits => bits;

        /// <summary>
        /// Gets the width, in pixels, of this image.
        /// </summary>
        public int Width => width;

        /// <summary>
        /// Gets the height, in pixels, of this image.
        /// </summary>
        public int Height => height;

        #endregion
    }
}
