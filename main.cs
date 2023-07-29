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

            ExecuteApp(app);

            app.Shutdown();
        }

        static void ExecuteApp(OpenTKApp app)
        {
            app.Run(() =>
            {
                GL.ClearColor(0.2f, 0.3f, 0.4f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit);
            });

        }
    }
}
