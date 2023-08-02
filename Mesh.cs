
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace OpenTKBase
{
    public class Mesh
    {
        private PrimitiveType   primitive = PrimitiveType.Triangles;
        private List<Vector3>   vertices;
        private List<Vector3>   normals;
        private List<Color4>    colors;
        private List<int>       indices;


        public void SetIndices(List<int> indices)
        {
            this.indices = indices;
        }

        public List<int> GetIndices() => indices;

        public void SetVertices(List<Vector3> vertices)
        {
            this.vertices = vertices;
        }

        public List<Vector3> GetVertices() => vertices;

        public void SetNormals(List<Vector3> normals)
        {
            this.normals = normals;
        }

        public List<Vector3> GetNormals() => normals;

        public void SetColors(List<Color4> colors)
        {
            this.colors = colors;
        }

        public List<Color4> GetColors() => colors;

        public void RenderImmediate()
        {
            if (vertices is null) return;

            GL.Begin(primitive);

            if (indices == null)
            {
                for (int i = 0; i < vertices.Count; i++)
                {
                    if (colors != null) GL.Color4(colors[i]);
                    if (normals != null) GL.Normal3(normals[i]);
                    GL.Vertex3(vertices[i]);
                }
            }
            else
            {
                foreach (var index in indices)
                {
                    if (colors != null) GL.Color4(colors[index]);
                    if (normals != null) GL.Normal3(normals[index]);
                    GL.Vertex3(vertices[index]);
                }
            }

            GL.End();
        }
    }
}
