using OpenTK.Graphics.OpenGL;
using System;

namespace SDLBase
{
    public class OpenTKProgram
    {
        public static void Main()
        {
            OpenTKApp app = new OpenTKApp(1280, 720, "OpenTK Base Application");

            app.Initialize();

            ExecuteApp_TriangleImmediate(app);

            app.Shutdown();
        }

        static void ExecuteApp_Clear(OpenTKApp app)
        {
            app.Run(() =>
            {
                GL.ClearColor(0.2f, 0.3f, 0.4f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit);
            });

        }

        static void ExecuteApp_TriangleImmediate(OpenTKApp app)
        {
            app.Run(() =>
            {
                GL.ClearColor(0.2f, 0.4f, 0.2f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                
                //GL.LoadIdentity();

                //GL.Translate(-1.5f, 0.0f, -6.0f);

                //GL.Color4(1.0f, 0.0f, 0.0f, 1.0f);

                GL.Begin(PrimitiveType.Triangles);
                    GL.Vertex2(0.0f, 0.0f);
                    GL.Vertex2(0.0f, 1.0f);
                    GL.Vertex2(1.0f, 0.0f);
                GL.End();
            });
        }

        static void ExecuteApp_TriangleCore(OpenTKApp app)
        {
            app.Run(() =>
            {
                GL.ClearColor(0.2f, 0.3f, 0.4f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                GL.LoadIdentity();

                GL.Translate(-1.5f, 0.0f, -6.0f);

                GL.PointSize(10.0f);
                GL.Color4(1.0f, 0.0f, 0.0f, 1.0f);

                GL.Begin(PrimitiveType.Points);
                GL.Vertex3(0.0f, 1.0f, -0.5f);
                GL.Vertex3(-1.0f, -1.0f, -0.5f);
                GL.Vertex3(1.0f, -1.0f, -0.5f);
                GL.End();
            });

        }
    }
}
