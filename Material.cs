
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace OpenTKBase
{
    public class Material
    {
        public Color4 color = Color4.White;
        public Shader shader;

        public Material(Shader shader)
        {
            this.shader = shader;
        }

        public void SetImmediate()
        {
            GL.Color4(color);
        }
    }
}
