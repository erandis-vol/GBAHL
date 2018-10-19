using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;

namespace GBAHL.Drawing.OpenGL
{
    /// <summary>
    /// Represents an OpenGL texture.
    /// </summary>
    public class Texture : IDisposable
    {
        private int handle;
        private bool isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture"/> class with the specified size.
        /// </summary>
        /// <param name="width">The width, in pixels, of the texture.</param>
        /// <param name="height">The height, in pixels, of the texture.</param>
        public Texture(int width, int height)
        {
            Width = width;
            Height = height;

            GL.GenTextures(1, out handle);

            GL.BindTexture(TextureTarget.Texture2D, handle);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Nearest);
        }

        ~Texture()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases all resources used by the texture.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    GL.DeleteTextures(1, ref handle);
                }

                isDisposed = true;
            }
        }

        /// <summary>
        /// Binds the texture.
        /// </summary>
        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, handle);
        }

        /// <summary>
        /// Sets the image data for the texture.
        /// </summary>
        /// <param name="data">The data to be set.</param>
        public void SetData(Color4[] data)
        {
            GL.BindTexture(TextureTarget.Texture2D, handle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height,
                0, PixelFormat.Rgba, PixelType.Float, data);
        }

        /// <summary>
        /// Gets the width in pixels.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the height in pixels.
        /// </summary>
        public int Height { get; }
    }
}
