using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GBAHL.Drawing.OpenGL
{
    public class SpriteRenderer : IDisposable
    {
        private static float[] vertexes = new[] {
        //   X     Y     U     V
            0.0f, 0.0f, 0.0f, 0.0f,
            1.0f, 0.0f, 1.0f, 0.0f,
            1.0f, 1.0f, 1.0f, 1.0f,
            0.0f, 1.0f, 0.0f, 1.0f
        };

        private static uint[] indexes = new[] {
            0u, 1u, 2u,
            2u, 3u, 0u
        };

        private Matrix4 model;
        private Matrix4 view;
        private Matrix4 projection;

        private IGraphicsContext context;
        private Texture texture;
        private Shader shader;
        private VertexArray<float> array;

        private Sprite sprite;

        private bool isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteRenderer"/> class.
        /// </summary>
        public SpriteRenderer()
        {
            // Initialize the texture to nothing
            texture = null;

            // Initialize the VAO
            array = new VertexArray<float>(vertexes, indexes);

            // Initialize shader
            shader = new Shader();
            shader.Attach(Encoding.UTF8.GetString(Properties.Resources.SpriteFragShader), ShaderType.FragmentShader);
            shader.Attach(Encoding.UTF8.GetString(Properties.Resources.SpriteVertShader), ShaderType.VertexShader);
            shader.Link();
        }

        ~SpriteRenderer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases all resources used by the <see cref="SpriteRenderer"/>.
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
                texture?.Dispose();                
                shader?.Dispose();
                array?.Dispose();

                isDisposed = true;
            }
        }

        [DebuggerNonUserCode]
        private void EnsureNotDisposed()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(SpriteRenderer));
            }
        }

        /// <summary>
        /// Draws the current sprite.
        /// </summary>
        public void Draw()
        {
            EnsureNotDisposed();

            if (texture != null)
            {
                // Use the shader
                shader.Use();

                // Bind the texture
                texture.Bind();

                // Bind the vertexes
                array.Bind();

                // Draw the texture
                GL.DrawElements(BeginMode.Triangles, array.Length, DrawElementsType.UnsignedInt, 0);
            }
        }

        /// <summary>
        /// Gets or sets the sprite.
        /// </summary>
        public Sprite Sprite
        {
            get => sprite;
            set
            {
                if (texture != null)
                {
                    texture.Dispose();
                    texture = null;
                }

                if (sprite != null)
                {
                    var w = sprite.Width << 3;
                    var h = sprite.Height << 3;

                    var data = new Color4[w * h];

                    var tileset = sprite.Tileset;
                    var palette = sprite.Palette;
                    var columns = sprite.Width;

                    for (int i = 0; i < tileset.Length; i++)
                    {
                        var x = i % columns;
                        var y = i / columns;
                        ref var tile = ref tileset[i];

                        for (int j = 0; j < 8; j++)
                        {
                            for (int k = 0; k < 8; k++)
                            {
                                data[(x * 8 + k) + (y * 8 * j) * w] = palette[tile[k, j]].ToColor4();
                            }
                        }
                    }

                    if (sprite.HasExtraTiles)
                    {
                        // TODO?
                    }

                    texture = new Texture(w, h);
                }
            }
        }
    }
}
