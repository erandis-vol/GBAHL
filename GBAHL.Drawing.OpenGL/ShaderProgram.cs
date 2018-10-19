using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GBAHL.Drawing.OpenGL
{
    public class ShaderProgram : IDisposable
    {
        private int program;
        private List<int> shaders = new List<int>();

        private bool isDisposed = false;

        public ShaderProgram()
        {
            program = GL.CreateProgram();
        }

        ~ShaderProgram()
        {
            Dispose();
        }

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
                throw new ObjectDisposedException(nameof(ShaderProgram));
            }
        }

        public void AddShader(string source, ShaderType type)
        {
            EnsureNotDisposed();

            var shader = GL.CreateShader(type);
            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);

            // TODO: Check shader status, throw on compile error

            GL.AttachShader(program, shader);
        }

        public void Link()
        {
            EnsureNotDisposed();

            GL.LinkProgram(program);
            
            // TODO: Check program status, throw on link error
        }

        public void Bind()
        {
            EnsureNotDisposed();

            GL.UseProgram(program);
        }

        public void SetUniform(string name, ref Matrix4 matrix)
        {
            EnsureNotDisposed();

            // NOTE: Can only be called after the program has been bound
            GL.UniformMatrix4(GL.GetUniformLocation(program, name), false, ref matrix);
        }
    }
}
