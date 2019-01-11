using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Diagnostics;
using System.Linq;

namespace GBAHL.Drawing.OpenGL
{
    /// <summary>
    /// Represents an OpenGL texture.
    /// </summary>
    internal class Texture : IDisposable
    {
        private int texture;
        private bool isDisposed = false;

        public Texture(int width, int height)
        {
            Width = width;
            Height = height;

            GL.GenTextures(1, out texture);

            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Nearest);
        }

        public Texture(int width, int height, Color4[] data)
            : this(width, height)
        {
            SetData(data);
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                GL.DeleteTextures(1, ref texture);
                isDisposed = true;
            }
        }

        [DebuggerNonUserCode]
        private void EnsureNotDisposed()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(Texture));
            }
        }

        public void Bind()
        {
            EnsureNotDisposed();
            GL.BindTexture(TextureTarget.Texture2D, texture);
        }

        public void SetData(Color4[] data)
        {
            EnsureNotDisposed();

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                Width,
                Height,
                0,
                PixelFormat.Rgba,
                PixelType.Float,
                data
            );
        }

        public void SetData(Bgr555[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            SetData(data.Select(x => x.ToColor4()).ToArray());
        }

        public int Width { get; }

        public int Height { get; }
    }
}
