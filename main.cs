using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTKBase;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace SDLBase
{
    public class OpenTKProgram
    {
        public static void Main()
        {
            OpenTKApp app = new OpenTKApp(1280, 720, "OpenTK Base Application");

            app.Initialize();
            app.LockMouse(true);

            ExecuteApp_EngineMesh(app);

            app.Shutdown();
        }

        static Mesh CreatePyramid(int nSides, float baseRadius, float height)
        {
            var pos = new List<Vector3>();
            var colors = new List<Color4>();

            Mesh mesh = new Mesh();

            float angleInc = MathF.PI * 2.0f / nSides;
            for (int i = 0; i < nSides; i++)
            {
                pos.Add(new Vector3((float)(baseRadius * MathF.Cos(angleInc * i)), 0.0f, (float)(baseRadius * MathF.Sin(angleInc * i))));
                pos.Add(new Vector3((float)(baseRadius * MathF.Cos(angleInc * (i + 1))), 0.0f, (float)(baseRadius * MathF.Sin(angleInc * (i + 1)))));
                pos.Add(new Vector3(0.0f, height, 0.0f));

                colors.Add(Color4.Yellow);
                colors.Add(Color4.Red);
                colors.Add(Color4.Green);

                pos.Add(new Vector3((float)(baseRadius * MathF.Cos(angleInc * i)), 0.0f, (float)(baseRadius * MathF.Sin(angleInc * i))));
                pos.Add(new Vector3((float)(baseRadius * MathF.Cos(angleInc * (i + 1))), 0.0f, (float)(baseRadius * MathF.Sin(angleInc * (i + 1)))));
                pos.Add(new Vector3(0.0f, 0.0f, 0.0f));

                colors.Add(Color4.Green);
                colors.Add(Color4.Cyan);
                colors.Add(Color4.Blue);
            }

            mesh.SetVertices(pos);
            mesh.SetColor0(colors);

            return mesh;
        }

        static Mesh CreatePlane(float sizeX, float sizeZ)
        {
            float sx = sizeX * 0.5f;
            float sz = sizeZ * 0.5f;

            var pos = new List<Vector3>();

            Mesh mesh = new Mesh();

            pos.Add(new Vector3(-sx, 0.0f, -sz));
            pos.Add(new Vector3(-sx, 0.0f,  sz));
            pos.Add(new Vector3( sx, 0.0f,  sz));
            
            pos.Add(new Vector3(-sx, 0.0f, -sz));
            pos.Add(new Vector3( sx, 0.0f,  sz));
            pos.Add(new Vector3( sx, 0.0f, -sz));

            mesh.SetVertices(pos);

            return mesh;
        }

        static void ExecuteApp_EngineMesh(OpenTKApp app)
        {
            // Create ground
            Mesh mesh = CreatePlane(60.0f, 60.0f);

            Material material = new Material();
            material.color = Color4.DarkGreen;

            GameObject go = new GameObject();
            go.transform.position = new Vector3(0, 0, 0);
            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.mesh = mesh;
            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mr.material = material;

            // Create pyramid
            mesh = CreatePyramid(3, 1.5f, 3.0f);

            material = new Material();
            material.color = Color4.Yellow;

            go = new GameObject();
            go.transform.position = new Vector3(0, 0, -15);
            mf = go.AddComponent<MeshFilter>();
            mf.mesh = mesh;
            mr = go.AddComponent<MeshRenderer>();
            mr.material = material;

            // Create camera
            GameObject cameraObject = new GameObject();
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.transform.position = new Vector3(0.0f, 2.0f, 0.0f);
            camera.ortographic = false;
            FirstPersonController fps = cameraObject.AddComponent<FirstPersonController>();

            // Create pipeline
            RPSIM renderPipeline = new RPSIM();

            app.Run(() =>
            {
                app.Render(renderPipeline);
            });
        }
    }
}
