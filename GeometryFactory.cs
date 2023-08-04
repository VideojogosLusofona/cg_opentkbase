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

        public static Mesh AddCylinder(float radius, float height, int sides = 16)
        {
            Mesh ret = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<int> indices = new List<int>();

            float angle = 0.0f;
            float angleInc = 2.0f * MathF.PI / sides;

            Vector3 topVertex = new Vector3(0.0f, height, 0.0f);
            for (int i = 0; i < sides; i++)
            {
                Vector3 n1 = new Vector3(MathF.Cos(angle), 0.0f, MathF.Sin(angle));
                Vector3 n2 = new Vector3(MathF.Cos(angle + angleInc), 0.0f, MathF.Sin(angle + angleInc));
                Vector3 n3 = n1;
                Vector3 n4 = n2;
                Vector3 p1 = n1 * radius;
                Vector3 p2 = n2 * radius;
                Vector3 p3 = p1 + Vector3.UnitY * height;
                Vector3 p4 = p2 + Vector3.UnitY * height;

                vertices.Add(Vector3.Zero); normals.Add(-Vector3.UnitY);
                vertices.Add(p1); normals.Add(-Vector3.UnitY);
                vertices.Add(p2); normals.Add(-Vector3.UnitY);

                vertices.Add(p1); normals.Add(n1);
                vertices.Add(p3); normals.Add(n3);
                vertices.Add(p4); normals.Add(n4);

                vertices.Add(p1); normals.Add(n1);
                vertices.Add(p4); normals.Add(n4);
                vertices.Add(p2); normals.Add(n2);

                vertices.Add(topVertex); normals.Add(Vector3.UnitY);
                vertices.Add(p4); normals.Add(Vector3.UnitY);
                vertices.Add(p3); normals.Add(Vector3.UnitY);

                angle += angleInc;
            }

            ret.SetVertices(vertices);
            ret.SetNormals(normals);

            return ret;
        }
    }
}
