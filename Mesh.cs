﻿
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
        private List<Color4>    colors;
        private List<uint>      indices;

        private bool            dirty = true;

        private int             vbo = -1;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct VertexData
        {
            public Vector3  position;
            public Vector3  normal;
            public Color4   color;

            public static readonly int SizeInBytes = Marshal.SizeOf<VertexData>();
        }

        ~Mesh()
        {
            if (vbo != -1)
            {
                GL.DeleteBuffer(vbo);
                vbo = -1;
            }
        }
        
        public void SetIndices(List<uint> indices)
        {
            this.indices = indices;
        }

        public List<uint> GetIndices() => indices;

        public void SetVertices(List<Vector3> vertices)
        {
            this.vertices = vertices;
            dirty = true;
        }

        public List<Vector3> GetVertices() => vertices;

        public void SetNormals(List<Vector3> normals)
        {
            this.normals = normals;
            dirty = true;
        }

        public List<Vector3> GetNormals() => normals;

        public void SetColors(List<Color4> colors)
        {
            this.colors = colors;
            dirty = true;
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
                    if (colors != null) GL.Color4(colors[(int)index]);
                    if (normals != null) GL.Normal3(normals[(int)index]);
                    GL.Vertex3(vertices[(int)index]);
                }
            }

            GL.End();
        }

        public void Render(Material material)
        {
            Shader shader = material.shader;
            if (shader == null) return;

            if (dirty)
            {
                Update();
            }

            int vao = GetVAO(shader);

            shader.Set();
            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Count);
        }

        public void Update()
        {
            if (vbo != -1)
            {
                // Guarantee that this buffer is not in use
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DeleteBuffer(vbo);
            }

            vbo = GL.GenBuffer();

            // Marshalling
            VertexData[] marshallData = new VertexData[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
            {
                marshallData[i].position = vertices[i];
                if (normals != null) marshallData[i].normal = normals[i];
                if (colors != null) marshallData[i].color = colors[i];
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, VertexData.SizeInBytes * vertices.Count, marshallData, BufferUsageHint.StaticDraw);

            dirty = false;
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
            if (colors != null) SetupVAO(shader, "color", 4, VertexAttribPointerType.Float, false);

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
