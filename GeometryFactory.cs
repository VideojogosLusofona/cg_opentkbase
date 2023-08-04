using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace OpenTKBase
{
    public static class GeometryFactory
    {
        public static Mesh AddPlane(float sizeX, float sizeZ)
        {
            Mesh            ret = new Mesh();
            List<Vector3>   vertices = new List<Vector3>();
            List<Vector3>   normals = new List<Vector3>();
            List<int>       indices = new List<int>();

            float sx = sizeX * 0.5f;
            float sz = sizeZ * 0.5f;

            vertices.Add(new Vector3(-sx, 0.0f, -sz)); normals.Add(new Vector3(0.0f, 1.0f, 0.0f));
            vertices.Add(new Vector3(-sx, 0.0f,  sz)); normals.Add(new Vector3(0.0f, 1.0f, 0.0f));
            vertices.Add(new Vector3( sx, 0.0f,  sz)); normals.Add(new Vector3(0.0f, 1.0f, 0.0f));

            vertices.Add(new Vector3(-sx, 0.0f, -sz)); normals.Add(new Vector3(0.0f, 1.0f, 0.0f));
            vertices.Add(new Vector3( sx, 0.0f,  sz)); normals.Add(new Vector3(0.0f, 1.0f, 0.0f));
            vertices.Add(new Vector3( sx, 0.0f, -sz)); normals.Add(new Vector3(0.0f, 1.0f, 0.0f));

            ret.SetVertices(vertices);
            ret.SetNormals(normals);

            return ret;
        }

        public static Mesh AddCylinder(float radius, float height, int subdivs = 16)
        {
            Mesh ret = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<uint> indices = new List<uint>();

            float angle = 0.0f;
            float angleInc = 2.0f * MathF.PI / subdivs;

            // Bottom vertex (index = 0), we use some variables to keep track of when the indices
            // for different parts start
            uint bottomVertexIndex = (uint)vertices.Count;
            vertices.Add(Vector3.Zero); normals.Add(-Vector3.UnitY);

            // Bottom cap - Insert the bottom edge pointing down
            uint startBottom = (uint)vertices.Count;
            for (int i = 0; i < subdivs; i++)
            {
                vertices.Add(new Vector3(radius * MathF.Cos(angle), 0.0f, radius * MathF.Sin(angle)));
                normals.Add(-Vector3.UnitY);

                angle += angleInc;
            }
            uint endBottom = (uint)vertices.Count;
            uint bottomCount = endBottom - startBottom;

            // Add indices for the bottom cap
            for (uint i = 0; i < subdivs; i++)
            {
                indices.Add(bottomVertexIndex);
                indices.Add(startBottom + i);
                indices.Add(startBottom + (i + 1) % bottomCount);
            }

            // Side part (vertices/normals)
            angle = 0.0f;
            uint startSide = (uint)vertices.Count;
            for (int i = 0; i < subdivs; i++)
            {
                Vector3 n = new Vector3(MathF.Cos(angle), 0.0f, MathF.Sin(angle));
                Vector3 p0 = n * radius;
                Vector3 p1 = p0 + Vector3.UnitY * height;

                vertices.Add(p0); normals.Add(n);
                vertices.Add(p1); normals.Add(n);

                angle += angleInc;
            }
            uint endSide = (uint)vertices.Count;
            uint sideCount = endSide - startSide;

            // Side part, indices
            for (uint i = 0; i < subdivs; i++)
            {
                indices.Add(startSide + (i * 2));
                indices.Add(startSide + ((i * 2) + 1) % sideCount);
                indices.Add(startSide + ((i * 2) + 2 + 1) % sideCount);

                indices.Add(startSide + (i * 2));
                indices.Add(startSide + ((i * 2) + 2 + 1) % sideCount);
                indices.Add(startSide + ((i * 2) + 2) % sideCount);
            }

            // Top vertex
            uint topVertexIndex = (uint)vertices.Count;
            vertices.Add(Vector3.UnitY * height); normals.Add(Vector3.UnitY);

            // Top cap - Insert the top edge pointing up
            uint startTop = (uint)vertices.Count;
            for (uint i = 0; i < subdivs; i++)
            {
                vertices.Add(new Vector3(radius * MathF.Cos(angle), height, radius * MathF.Sin(angle)));
                normals.Add(Vector3.UnitY);

                angle += angleInc;
            }
            uint endTop = (uint)vertices.Count;
            uint topCount = endTop - startTop;

            // Add indices for the top cap
            for (uint i = 0; i < subdivs; i++)
            {
                indices.Add(topVertexIndex);
                indices.Add(startTop + (i + 1) % topCount);
                indices.Add(startTop + i);
            }

            ret.SetVertices(vertices);
            ret.SetNormals(normals);
            ret.SetIndices(indices);

            return ret;
        }

        public static Mesh AddSphere(float radius, int sides = 16)
        {
            Mesh ret = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<uint> indices = new List<uint>();

            int sidesY = sides / 2;

            float angleY = 0.0f;
            float angleIncY = MathF.PI / (sidesY - 1);

            float angleXZ = 0.0f;
            float angleIncXZ = 2.0f * MathF.PI / sides;

            for (int y = 0; y < sidesY; y++)
            {
                angleXZ = 0.0f;

                for (int xz = 0; xz < sides; xz++)
                {
                    Vector3 n = new Vector3(MathF.Sin(angleY) * MathF.Cos(angleXZ), MathF.Cos(angleY), MathF.Sin(angleY) * MathF.Sin(angleXZ));
                    Vector3 p = n * radius;

                    vertices.Add(p);
                    normals.Add(n);

                    angleXZ += angleIncXZ;
                }

                angleY += angleIncY;
            }

            for (int y = 0; y < sidesY - 1; y++)
            {
                int yIndex0 = y * sides;
                int yIndex1 = (y + 1) * sides;

                for (int xz = 0; xz < sides; xz++)
                {
                    indices.Add((uint)(yIndex0 + xz));
                    indices.Add((uint)(yIndex0 + (xz + 1) % sides));
                    indices.Add((uint)(yIndex1 + (xz + 1) % sides));

                    indices.Add((uint)(yIndex0 + xz));
                    indices.Add((uint)(yIndex1 + (xz + 1) % sides));
                    indices.Add((uint)(yIndex1 + xz));
                }
            }

            ret.SetVertices(vertices);
            ret.SetNormals(normals);
            ret.SetIndices(indices);

            return ret;
        }
    }
}
