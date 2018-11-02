using System;

namespace GBAHL.Drawing.OpenGL
{
    public class ShaderCompileException : Exception
    {
        public ShaderCompileException(string message)
            : base(message)
        {

        }
    }
}
