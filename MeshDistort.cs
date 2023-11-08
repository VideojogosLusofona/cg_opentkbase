using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenTKBase
{
    public class MeshDistort : Component
    {
        private List<Vector3> originalPos;
        private bool          init  = false;
        private float         angle = 0.0f;

        public override void Update()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            Mesh mesh = meshFilter.mesh;

            if (!init)
            {
                originalPos = mesh.GetVertices();

                init = true;
            }

            var normals = mesh.GetNormals();

            var newPos = new List<Vector3>();
            for (int i = 0; i < originalPos.Count; i++)
            {
                newPos.Add(originalPos[i] + normals[i] * MathF.Sin(angle + originalPos[i].Y * 1.234f + originalPos[i].X * 0.2134f + originalPos[i].Z * 2.315f) * 0.5f);
            }

            mesh.SetVertices(newPos);

            angle += 0.01f;
        }
    }
}
