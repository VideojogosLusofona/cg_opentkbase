
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace OpenTKBase
{
    public class Mesh
    {
        private PrimitiveType   primitive = PrimitiveType.Triangles;
        private List<Vector3>   vertices;
        private List<Vector3>   normals;
        private List<Vector4>   tangents;
        private List<Color4>    colors;
        private List<Vector2>   uvs;
        private List<uint>      indices;

        private bool            vertexDirty = true;
        private bool            indexDirty = false;

        private int             vbo = -1;
        private int             ibo = -1;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct VertexData
        {
            public Vector3  position;
            public Vector3  normal;
            public Vector4  tangent;
            public Color4   color;
            public Vector2  uv;

            public static readonly int SizeInBytes = Marshal.SizeOf<VertexData>();
        }

        ~Mesh()
        {
            if (vbo != -1)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DeleteBuffer(vbo);
                vbo = -1;
            }
            if (ibo != -1)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                GL.DeleteBuffer(ibo);
                ibo = -1;
            }
        }

        public void SetIndices(List<uint> indices)
        {
            this.indices = indices;
            indexDirty = true;
        }

        public List<uint> GetIndices() => indices;

        public void SetVertices(List<Vector3> vertices)
        {
            this.vertices = vertices;
            vertexDirty = true;
        }

        public List<Vector3> GetVertices() => vertices;

        public void SetNormals(List<Vector3> normals)
        {
            this.normals = normals;
            vertexDirty = true;
        }

        public List<Vector3> GetNormals() => normals;
        
        public void SetTangents(List<Vector4> tangents)
        {
            this.tangents = tangents;
            vertexDirty = true;
        }

        public List<Vector4> GetTangents() => tangents;

        public void SetColors(List<Color4> colors)
        {
            this.colors = colors;
            vertexDirty = true;
        }

        public List<Color4> GetColors() => colors;

        public void SetUVs(List<Vector2> uvs)
        {
            this.uvs = uvs;
            vertexDirty = true;
        }

        public List<Vector2> GetUVs() => uvs;

        public void ComputeNormalsAndTangentSpace()
        {
            var indices = this.indices;
            var vertices = this.vertices;
            var uv = this.uvs;

            List<Vector3> triangle_normals = new List<Vector3>();
            List<Vector3> triangle_tangents = new List<Vector3>();
            List<Vector3> triangle_binormals = new List<Vector3>();
            List<float> triangle_areas = new List<float>();
            List<List<int>> triangles_per_vertex = new List<List<int>>();
            for (int i = 0; i < vertices.Count; i++) triangles_per_vertex.Add(new List<int>());

            for (int i = 0; i < indices.Count; i += 3)
            {
                uint i0, i1, i2;
                i0 = indices[i];
                i1 = indices[i + 1];
                i2 = indices[i + 2];

                Vector3 v0, v1, v2;
                Vector2 uv0, uv1, uv2;
                v0 = vertices[(int)i0]; uv0 = uv[(int)i0];
                v1 = vertices[(int)i1]; uv1 = uv[(int)i1];
                v2 = vertices[(int)i2]; uv2 = uv[(int)i2];

                var side0 = v0 - v1;
                var side1 = v2 - v1;

                // Normal and triangle area
                Vector3 normal = Vector3.Cross(side1, side0);
                triangle_areas.Add(normal.Length * 0.5f);
                normal = normal.Normalized();
                triangle_normals.Add(normal);

                // Tangent space
                Vector2 delta0 = uv0 - uv1;
                Vector2 delta1 = uv2 - uv1;

                Vector3 tangent = (delta1.Y * side0 - delta0.Y * side1).Normalized();
                Vector3 binormal = (delta1.X * side0 - delta0.X * side1).Normalized();

                var tangent_cross = Vector3.Cross(tangent, binormal);
                if (Vector3.Dot(tangent_cross, normal) < 0.0f)
                {
                    tangent = -tangent;
                    binormal = -binormal;
                }

                triangle_binormals.Add(binormal);
                triangle_tangents.Add(tangent);

                triangles_per_vertex[(int)i0].Add((int)i / 3);
                triangles_per_vertex[(int)i1].Add((int)i / 3);
                triangles_per_vertex[(int)i2].Add((int)i / 3);
            }

            List<Vector3> normals = new List<Vector3>();
            List<Vector4> tangents = new List<Vector4>();
            for (int i = 0; i < vertices.Count; i++)
            {
                normals.Add(Vector3.Zero);
                tangents.Add(Vector4.Zero);
            }

            for (int i = 0; i < vertices.Count; i++)
            {
                Vector3 normal = Vector3.Zero;
                Vector3 tangent = Vector3.Zero;
                Vector3 binormal = Vector3.Zero;
                float sign = 1.0f;

                foreach (var index in triangles_per_vertex[i])
                {
                    normal += triangle_normals[index];
                    tangent += triangle_tangents[index];
                    binormal += triangle_binormals[index];
                }

                normal.Normalize();
                tangent.Normalize();
                binormal.Normalize();

                var tmp = Vector3.Cross(normal, tangent);
                if (Vector3.Dot(tmp, binormal) < 0) sign = -1.0f; else sign = 1.0f;

                normals[i] = normal;
                tangents[i] = new Vector4(tangent, sign);
            }

            SetNormals(normals);
            SetTangents(tangents);
        }

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
                    if (uvs != null) GL.TexCoord2(uvs[i]);
                    GL.Vertex3(vertices[i]);
                }
            }
            else
            {
                foreach (var index in indices)
                {
                    if (colors != null) GL.Color4(colors[(int)index]);
                    if (normals != null) GL.Normal3(normals[(int)index]);
                    if (uvs != null) GL.TexCoord2(uvs[(int)index]);
                    GL.Vertex3(vertices[(int)index]);
                }
            }

            GL.End();
        }

        public void Render(Material material)
        {
            // Retrieve shader, if there isn't a shader, do nothing
            Shader shader = material.shader;
            if (shader == null) return;

            // Update VBO, if data was modified
            if (vertexDirty) UpdateVertex();

            // Create or retrieve VAO
            int vao = GetVAO(shader);
            GL.BindVertexArray(vao);

            // Setup shader
            shader.Set(material);

            // Render
            if (indices == null)
            {
                GL.DrawArrays(primitive, 0, vertices.Count);
            }
            else
            {
                if (indexDirty) UpdateIndex();

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);

                GL.DrawElements(primitive, indices.Count, DrawElementsType.UnsignedInt, 0);
            }
        }

        public void UpdateVertex()
        {
            if (vbo != -1)
            {
                // Guarantee that this buffer is not in use
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DeleteBuffer(vbo);
                vbo = -1;
            }

            vbo = GL.GenBuffer();

            // Marshalling
            VertexData[] marshallData = new VertexData[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
            {
                marshallData[i].position = vertices[i];
                if (normals != null) marshallData[i].normal = normals[i];
                if (tangents != null) marshallData[i].tangent = tangents[i];
                if (colors != null) marshallData[i].color = colors[i];
                if (uvs  != null) marshallData[i].uv = uvs[i];
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, VertexData.SizeInBytes * vertices.Count, marshallData, BufferUsageHint.StaticDraw);

            vertexDirty = false;
        }
        public void UpdateIndex()
        {
            // This function assumes a VAO was already bound

            if (ibo != -1)
            {
                // Guarantee that this buffer is not in use
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                GL.DeleteBuffer(ibo);
                ibo = -1;
            }

            // Create IBO
            ibo = GL.GenBuffer();

            // Marshalling
            uint[] marshallData = indices.ToArray();

            // Bind buffer
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
            // Set data
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Count, marshallData, BufferUsageHint.StaticDraw);

            // Reset dirty flag
            indexDirty = false;
        }

        // Dictionary that contains the links between this mesh and any shader that's needed
        Dictionary<Shader, int> vaos = new Dictionary<Shader, int>();
        private int GetVAO(Shader shader)
        {
            // See if we already built this one
            int vao;
            if (vaos.TryGetValue(shader, out vao))
            {
                return vao;
            }

            // Create Vertex Array Object
            vao = GL.GenVertexArray();
            // Bind this VAO (so we can configure it)
            GL.BindVertexArray(vao);
            // Bind the VBO to this VAO
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            // Check if shader needs a specific attribute
            if (vertices != null) SetupVAO(shader, "position", 3, VertexAttribPointerType.Float, false);
            if (normals != null) SetupVAO(shader, "normal", 3, VertexAttribPointerType.Float, false);
            if (tangents != null) SetupVAO(shader, "tangent", 4, VertexAttribPointerType.Float, false);
            if (colors != null) SetupVAO(shader, "color", 4, VertexAttribPointerType.Float, false);
            if (uvs != null) SetupVAO(shader, "uv", 2, VertexAttribPointerType.Float, false);

            // Add it to our array
            vaos.Add(shader, vao);

            // Return it
            return vao;
        }

        // Helper function
        private void SetupVAO(Shader shader, string attributeName, int attrSize, VertexAttribPointerType dataType, bool normalize)
        {
            // Check if shader needs this attribute
            int layoutPos = shader.GetAttributePos(attributeName);
            if (layoutPos == -1) return;

            // Setup the link between the data
            GL.VertexAttribPointer(layoutPos, attrSize, dataType, normalize, VertexData.SizeInBytes, Marshal.OffsetOf<VertexData>(attributeName));
            // Enable this attribute
            GL.EnableVertexAttribArray(layoutPos);
        }
    }
}
