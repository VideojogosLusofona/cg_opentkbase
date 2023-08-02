
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace OpenTKBase
{
    public class Mesh
    {
        private PrimitiveType   primitive = PrimitiveType.Triangles;
        private List<Vector3>   vertex;
        private List<Color4>    color0;

        public void SetVertices(List<Vector3> vertices)
        {
            this.vertex = vertices;
        }

        public void SetColor0(List<Color4> colors)
        {
            this.color0 = colors;
        }

        public void RenderImmediate()
        {
            if (vertex is null) return;

            GL.Begin(primitive);

            for (int i = 0; i < vertex.Count; i++)
            {
                if (color0 != null) GL.Color4(color0[i]);
                GL.Vertex3(vertex[i]);
            }

            GL.End();
        }
    }
}
