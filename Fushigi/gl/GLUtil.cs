using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fushigi.gl
{
    public class GLUtil
    {
        public static void Label(GL gl, ObjectIdentifier type, uint id, string text)
        {
            var error = gl.GetError();
            if (error != GLEnum.NoError)
            {
                Console.WriteLine($"Unknown OpenGL Error: {error}");
                // throw new Exception();
            }

            gl.ObjectLabel(type, id, (uint)text.Length, text);

            error = gl.GetError();
            if (error != GLEnum.NoError)
            {
                Console.WriteLine($"OpenGL Shader Error: {error} when labeling {text}");
                // throw new Exception();
            }
        }
    }
}
