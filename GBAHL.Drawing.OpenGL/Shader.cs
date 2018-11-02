using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GBAHL.Drawing.OpenGL
{
    /// <summary>
    /// Represents an OpenGL shader program.
    /// </summary>
    public class Shader : IDisposable
    {
        private int program;
        private List<int> shaders = new List<int>();

        private bool isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Shader"/> class.
        /// </summary>
        public Shader()
        {
            program = GL.CreateProgram();
        }

        ~Shader()
        {
            Dispose();
        }

        /// <summary>
        /// Releases all resources used by the shader program.
        /// </summary>
        public void Dispose()
        {
            if (!isDisposed)
            {
                foreach (var shader in shaders)
                {
                    GL.DeleteShader(shader);
                }

                GL.DeleteProgram(program);

                isDisposed = true;
            }
        }

        [DebuggerNonUserCode]
        private void EnsureNotDisposed()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(Shader));
            }
        }

        /// <summary>
        /// Adds a shader to the program. Do not call once <see cref="Link"/> has been called.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <exception cref="ShaderCompileException">
        /// there was an error compiling the shader.
        /// </exception>
        public void Attach(string source, ShaderType type)
        {
            EnsureNotDisposed();

            // Create and compile shader
            var shader = GL.CreateShader(type);
            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);

            // Verify compile was successful
            int status;
            GL.GetShader(shader, ShaderParameter.CompileStatus, out status);

            if (status != 1)
            {
                var log = GL.GetShaderInfoLog(shader);
                throw new ShaderCompileException(log);
            }

            // Attach shader to program
            GL.AttachShader(program, shader);
            shaders.Add(shader);
        }

        /// <summary>
        /// Links the shader program.
        /// </summary>
        /// <exception cref="ShaderLinkException">
        /// there was an error linking the program.
        /// </exception>
        public void Link()
        {
            EnsureNotDisposed();

            // Link the shaders
            GL.LinkProgram(program);

            // Verify linking was successful
            int status;
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out status);

            if (status != 1)
            {
                var log = GL.GetProgramInfoLog(program);
                throw new ShaderLinkException(log);
            }
        }

        /// <summary>
        /// Use the program.
        /// </summary>
        public void Use()
        {
            EnsureNotDisposed();

            GL.UseProgram(program);
        }

        /// <summary>
        /// Sets a uniform value. Requires <see cref="Use"/> first.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="matrix"></param>
        public void SetUniform(string name, ref Matrix4 matrix)
        {
            EnsureNotDisposed();

            // NOTE: Can only be called after the program has been bound
            GL.UniformMatrix4(GL.GetUniformLocation(program, name), false, ref matrix);
        }
    }
}
