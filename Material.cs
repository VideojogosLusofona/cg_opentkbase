
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace OpenTKBase
{
    public class Material
    {
        public Color4 color = Color4.White;

        public void SetImmediate()
        {
            GL.Color4(color);
        }
    }
}
