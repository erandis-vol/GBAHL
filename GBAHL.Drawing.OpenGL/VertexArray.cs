using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GBAHL.Drawing.OpenGL
{
    /// <summary>
    /// Represents a vertex array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class VertexArray<T> : IDisposable where T : struct
    {
        private int array;
        private Buffer<T> vertexes;
        private Buffer<uint> indexes;

        private bool isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexArray{T}"/> clas.
        /// </summary>
        /// <param name="vertexData"></param>
        /// <exception cref="ArgumentNullException"><paramref name="vertexData"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="vertexData"/> is empty.</exception>
        public VertexArray(T[] vertexData)
            : this(vertexData, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexArray{T}"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="vertexData"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="vertexData"/> is empty.</exception>
        public VertexArray(T[] vertexData, uint[] indexData)
        {
            if (vertexData == null)
                throw new ArgumentNullException(nameof(vertexData));

            if (vertexData.Length == 0)
                throw new ArgumentException();

            GL.GenVertexArrays(1, out array);
            GL.BindVertexArray(array);

            vertexes = new Buffer<T>(BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw, vertexData);

            if (indexData == null || indexData.Length == 0)
            {
                indexData = new uint[vertexData.Length];
                for (uint i = 0; i < vertexData.Length; i++)
                    indexData[i] = i;
            }

            indexes = new Buffer<uint>(BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticRead, indexData);
            Length = indexData.Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexArray{T}"/> class.
        /// </summary>
        /// <param name="vertexData"></param>
        /// <exception cref="ArgumentNullException"><paramref name="vertexData"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="vertexData"/> is empty.</exception>
        public VertexArray(IEnumerable<T> vertexData)
            : this(vertexData, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexArray{T}"/> class.
        /// </summary>
        /// <param name="vertexData"></param>
        /// <param name="indexData"></param>
        /// <exception cref="ArgumentNullException"><paramref name="vertexData"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="vertexData"/> is empty.</exception>
        public VertexArray(IEnumerable<T> vertexData, IEnumerable<uint> indexData)
            : this(vertexData?.ToArray(), indexData?.ToArray())
        { }

        ~VertexArray()
        {
            Dispose();
        }

        /// <summary>
        /// Releases all resources used by the vertex array.
        /// </summary>
        public void Dispose()
        {
            if (!isDisposed)
            {
                vertexes.Dispose();
                indexes.Dispose();
                GL.DeleteVertexArrays(1, ref array);

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
        /// Binds the vertex array.
        /// </summary>
        public void Bind()
        {
            EnsureNotDisposed();
            GL.BindVertexArray(array);
        }

        /// <summary>
        /// Enables the specified vertex attribute. Requires binding.
        /// </summary>
        /// <param name="index">The index of the attribute.</param>
        /// <param name="size">The number of components. Must be in the range [1, 4].</param>
        /// <param name="type"></param>
        /// <param name="normalized"></param>
        /// <param name="stride"></param>
        /// <param name="offset"></param>
        public void EnableAttribute(
            int index,
            int size = 4,
            VertexAttribPointerType type = VertexAttribPointerType.Float,
            bool normalized = false,
            int stride = 0,
            int offset = 0)
        {
            EnsureNotDisposed();

            GL.VertexAttribPointer(index, size, type, normalized, stride, offset);
            GL.EnableVertexAttribArray(index);
        }

        /// <summary>
        /// Disables the specified vertex attribute. Requires binding.
        /// </summary>
        /// <param name="index">The index of the attribute.</param>
        public void DisableAttribute(int index)
        {
            EnsureNotDisposed();

            GL.DisableVertexAttribArray(index);
        }

        /// <summary>
        /// Gets the number of elements in the array.
        /// </summary>
        public int Length { get; }
    }
}
