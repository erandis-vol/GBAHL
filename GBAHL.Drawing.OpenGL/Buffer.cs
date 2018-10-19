using OpenTK.Graphics.OpenGL4;
using System;

namespace GBAHL.Drawing.OpenGL
{
    /// <summary>
    /// Represents an OpenGL buffer object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Buffer<T> : IDisposable where T : struct
    {
        private int buffer;
        private bool isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Buffer{T}"/> class.
        /// </summary>
        public Buffer()
        {
            GL.CreateBuffers(1, out buffer);
        }

        /// <summary>
        /// Releases all resources used by the buffer.
        /// </summary>
        public void Dispose()
        {
            if (!isDisposed)
            {
                GL.DeleteBuffers(1, ref buffer);
                isDisposed = true;
            }
        }

        /// <summary>
        /// Binds the buffer.
        /// </summary>
        /// <param name="target"></param>
        public void Bind(BufferTarget target)
        {
            GL.BindBuffer(target, buffer);
        }

        /// <summary>
        /// Sets the buffer's data.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="data"></param>
        /// <param name="hint"></param>
        public void SetData(BufferTarget target, T[] data, BufferUsageHint hint)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            GL.BindBuffer(target, buffer);
            GL.BufferData(target, data.Length, data, hint);
        }
    }
}
