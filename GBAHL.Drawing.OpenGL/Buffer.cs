using OpenTK.Graphics.OpenGL4;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GBAHL.Drawing.OpenGL
{
    /// <summary>
    /// Represents an OpenGL buffer object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class Buffer<T> : IDisposable where T : struct
    {
        private int buffer;
        private bool isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Buffer{T}"/> class.
        /// </summary>
        public Buffer(BufferTarget target, BufferUsageHint usage)
        {
            GL.GenBuffers(1, out buffer);

            Target = target;
            Usage = usage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Buffer{T}"/> class.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="usage"></param>
        /// <param name="data"></param>
        public Buffer(BufferTarget target, BufferUsageHint usage, T[] data)
            : this(target, usage)
        {
            Bind();
            SetData(data);
        }

        ~Buffer()
        {
            Dispose();
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

        [DebuggerNonUserCode]
        private void EnsureNotDisposed()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(Buffer<T>));
            }
        }

        /// <summary>
        /// Binds the buffer.
        /// </summary>
        /// <param name="target"></param>
        public void Bind()
        {
            GL.BindBuffer(Target, buffer);
        }

        /// <summary>
        /// Sets the buffer's data. Requires binding.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="data"></param>
        /// <param name="usage"></param>
        public void SetData(T[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (data.Length == 0)
                throw new ArgumentException();

            EnsureNotDisposed();

            GL.BufferData(Target, Marshal.SizeOf(typeof(T)) * data.Length, data, Usage);
            Size = data.Length;
        }

        /// <summary>
        /// Gets the buffer target.
        /// </summary>
        public BufferTarget Target { get; }

        /// <summary>
        /// Gets the buffer usage hint.
        /// </summary>
        public BufferUsageHint Usage { get; }

        /// <summary>
        /// Gets the number of elements in the buffer.
        /// </summary>
        public int Size { get; private set; } = 0;
    }
}
