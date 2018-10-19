using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Text;

namespace GBAHL.Drawing.OpenGL
{
    public class SpriteGL
    {
        private static readonly int[] Indexes = { 0, 1, 2, 2, 3, 0 };

        private Texture imageTexture;
        private Texture colorTexture;
        private Buffer<float> vertexBuffer;
        private Buffer<int> indexBuffer;
        private ShaderProgram shaderProgram;

        private bool isDisposed = false;

        /// <summary>
        /// Initalizes a new instance of the <see cref="SpriteGL"/> class.
        /// </summary>
        public SpriteGL(Sprite sprite)
        {
            Width = sprite.Width * 8;
            Height = sprite.Height * 8;

            // ----------------------------------------------------------------
            // Create the image texture
            imageTexture = new Texture(Width, Height);
            // TODO

            // ----------------------------------------------------------------
            // Create the color texture
            colorTexture = new Texture(256, 1);
            // TODO

            // ----------------------------------------------------------------
            // Create the vertex buffer
            var w = (float)Width;
            var h = (float)Height;

            var vertexes = new float[] {
            //  X  Y  U  V
                0, 0, 0, 0,
                w, 0, 1, 0,
                w, h, 1, 1,
                0, h, 0, 1
            };

            vertexBuffer = new Buffer<float>();
            vertexBuffer.SetData(BufferTarget.ArrayBuffer, vertexes, BufferUsageHint.DynamicDraw);

            // ----------------------------------------------------------------
            // Create the index buffer
            indexBuffer = new Buffer<int>();
            indexBuffer.SetData(BufferTarget.ElementArrayBuffer, Indexes, BufferUsageHint.StaticRead);

            // ----------------------------------------------------------------
            // Create the shader program
            shaderProgram = new ShaderProgram();
            shaderProgram.AddShader(Encoding.UTF8.GetString(Properties.Resources.SpriteVertexShader), ShaderType.VertexShader);
            shaderProgram.AddShader(Encoding.UTF8.GetString(Properties.Resources.SpriteFragmentShader), ShaderType.FragmentShader);
            shaderProgram.Link();
        }

        ~SpriteGL()
        {
            Dispose();
        }

        /// <summary>
        /// Releases all resources used by this <see cref="SpriteGL"/>.
        /// </summary>
        public void Dispose()
        {
            if (!isDisposed)
            {
                imageTexture.Dispose();
                colorTexture.Dispose();
                vertexBuffer.Dispose();
                indexBuffer.Dispose();
                shaderProgram.Dispose();

                isDisposed = true;
            }
        }

        /// <summary>
        /// Gets the width, in pixels, of the texture.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the height, in pixels, of the texture.
        /// </summary>
        public int Height { get; }
    }
}
