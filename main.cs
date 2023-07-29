using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;

namespace SDLBase
{
    public class OpenTKProgram
    {
        public static void Main()
        {
            OpenTKApp app = new OpenTKApp(1280, 720, "OpenTK Base Application");

            app.Initialize();

            ExecuteApp_TrianglesImmediateTransform(app);

            app.Shutdown();
        }

        static void ExecuteApp_TrianglesImmediateDepth(OpenTKApp app)
        {
            // Activate depth testing
            GL.Enable(EnableCap.DepthTest);
            // Set the test function
            GL.DepthFunc(DepthFunction.Lequal);
            // Enable depth write
            GL.DepthMask(true);

            float depthBright = 0.5f;
            float depthBlue = 0.25f;

            app.Run(() =>
            {
                // Clear color buffer and the depth buffer
                GL.ClearColor(0.2f, 0.4f, 0.2f, 1.0f);
                GL.ClearDepth(1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.Begin(PrimitiveType.Triangles);

                GL.Color4(1.0f, 0.0f, 0.0f, 1.0f);
                GL.Vertex3(0.0f, 0.5f, depthBright);
                GL.Color4(1.0f, 1.0f, 0.0f, 1.0f);
                GL.Vertex3(0.5f, -0.5f, depthBright);
                GL.Color4(0.0f, 1.0f, 0.0f, 1.0f);
                GL.Vertex3(-0.5f, -0.5f, depthBright);

                GL.Color4(0.0f, 0.5f, 1.0f, 1.0f);
                GL.Vertex3(0.2f, 0.6f, depthBlue);
                GL.Color4(0.8f, 0.4f, 1.0f, 1.0f);
                GL.Vertex3(0.7f, -0.4f, depthBlue);
                GL.Color4(0.0f, 1.0f, 1.0f, 1.0f);
                GL.Vertex3(-0.3f, -0.4f, depthBlue);

                GL.End();
            });
        }

        static void ExecuteApp_TrianglesImmediateDepthBlend(OpenTKApp app)
        {
            // Set depth test flags
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.DepthMask(true);
            // Set blend operation
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            float depthBright = 0.5f;

            app.Run(() =>
            {
                // Clear color buffer and the depth buffer
                GL.ClearColor(0.2f, 0.4f, 0.2f, 1.0f);
                GL.ClearDepth(1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.Begin(PrimitiveType.Triangles);

                GL.Color4(1.0f, 0.0f, 0.0f, 1.0f);
                GL.Vertex3(0.0f, 0.5f, depthBright);
                GL.Color4(1.0f, 1.0f, 0.0f, 1.0f);
                GL.Vertex3(0.5f, -0.5f, depthBright);
                GL.Color4(0.0f, 1.0f, 0.0f, 1.0f);
                GL.Vertex3(-0.5f, -0.5f, depthBright);

                GL.End();
            });

        }


        static void ExecuteApp_TrianglesImmediateTransform(OpenTKApp app)
        {
            // Activate depth testing
            GL.Enable(EnableCap.DepthTest);
            // Set the test function
            GL.DepthFunc(DepthFunction.Lequal);
            // Enable depth write
            GL.DepthMask(true);

            float   angle = 0.0f;
            Matrix4 worldMatrix;

            app.Run(() =>
            {
                // Clear color buffer and the depth buffer
                GL.ClearColor(0.2f, 0.4f, 0.2f, 1.0f);
                GL.ClearDepth(1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                worldMatrix = Matrix4.CreateRotationZ(angle);
                angle += 0.1f;

                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadMatrix(ref worldMatrix);

                GL.Begin(PrimitiveType.Triangles);

                GL.Color4(1.0f, 0.0f, 0.0f, 1.0f);
                GL.Vertex3(0.0f, 0.5f, 0.5f);
                GL.Color4(1.0f, 1.0f, 0.0f, 1.0f);
                GL.Vertex3(0.5f, -0.5f, 0.5f);
                GL.Color4(0.0f, 1.0f, 0.0f, 1.0f);
                GL.Vertex3(-0.5f, -0.5f, 0.5f);

                GL.End();
            });
        }
    }
}
