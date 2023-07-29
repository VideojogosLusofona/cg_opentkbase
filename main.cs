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

            ExecuteApp_TrianglesImmediateColorPerVertexBlend(app);

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
                
                GL.Begin(PrimitiveType.Triangles);
                    GL.Vertex3( 0.0f,  0.5f, 0.0f);
                    GL.Vertex3( 0.5f, -0.5f, 0.0f);
                    GL.Vertex3(-0.5f, -0.5f, 0.0f);
                GL.End();
            });
        }

        static void ExecuteApp_TriangleImmediateColor(OpenTKApp app)
        {
            app.Run(() =>
            {
                GL.ClearColor(0.2f, 0.4f, 0.2f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                GL.Color4(1.0f, 1.0f, 0.0f, 1.0f);

                GL.Begin(PrimitiveType.Triangles);
                GL.Vertex3(0.0f, 0.5f, 0.0f);
                GL.Vertex3(0.5f, -0.5f, 0.0f);
                GL.Vertex3(-0.5f, -0.5f, 0.0f);
                GL.End();
            });
        }


        static void ExecuteApp_TriangleImmediateColorPerVertex(OpenTKApp app)
        {
            app.Run(() =>
            {
                GL.ClearColor(0.2f, 0.4f, 0.2f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                GL.Begin(PrimitiveType.Triangles);
                GL.Color4(1.0f, 0.0f, 0.0f, 1.0f);
                GL.Vertex3(0.0f, 0.5f, 0.0f);
                GL.Color4(1.0f, 1.0f, 0.0f, 1.0f);
                GL.Vertex3(0.5f, -0.5f, 0.0f);
                GL.Color4(0.0f, 1.0f, 0.0f, 1.0f);
                GL.Vertex3(-0.5f, -0.5f, 0.0f);
                GL.End();
            });
        }

        static void ExecuteApp_TrianglesImmediateColorPerVertex(OpenTKApp app)
        {
            app.Run(() =>
            {
                GL.ClearColor(0.2f, 0.4f, 0.2f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                GL.Begin(PrimitiveType.Triangles);

                GL.Color4(1.0f, 0.0f, 0.0f, 1.0f);
                GL.Vertex3(0.0f, 0.5f, 0.0f);
                GL.Color4(1.0f, 1.0f, 0.0f, 1.0f);
                GL.Vertex3(0.5f, -0.5f, 0.0f);
                GL.Color4(0.0f, 1.0f, 0.0f, 1.0f);
                GL.Vertex3(-0.5f, -0.5f, 0.0f);

                GL.Color4(0.0f, 0.5f, 1.0f, 1.0f);
                GL.Vertex3(0.2f, 0.6f, 0.0f);
                GL.Color4(0.8f, 0.4f, 1.0f, 1.0f);
                GL.Vertex3(0.7f, -0.4f, 0.0f);
                GL.Color4(0.0f, 1.0f, 1.0f, 1.0f);
                GL.Vertex3(-0.3f, -0.4f, 0.0f);

                GL.End();
            });
        }

        static void ExecuteApp_TrianglesImmediateColorPerVertexBlend(OpenTKApp app)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            app.Run(() =>
            {
                GL.ClearColor(0.2f, 0.4f, 0.2f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                GL.Begin(PrimitiveType.Triangles);

                GL.Color4(1.0f, 0.0f, 0.0f, 1.0f);
                GL.Vertex3(0.0f, 0.5f, 0.0f);
                GL.Color4(1.0f, 1.0f, 0.0f, 1.0f);
                GL.Vertex3(0.5f, -0.5f, 0.0f);
                GL.Color4(0.0f, 1.0f, 0.0f, 1.0f);
                GL.Vertex3(-0.5f, -0.5f, 0.0f);

                GL.Color4(0.0f, 0.0f, 0.0f, 0.5f);
                GL.Vertex3(0.2f, 0.6f, 0.0f);
                GL.Vertex3(0.7f, -0.4f, 0.0f);
                GL.Vertex3(-0.3f, -0.4f, 0.0f);

                GL.End();
            });
        }
    }
}
