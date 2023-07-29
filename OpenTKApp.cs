
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;

namespace SDLBase
{
    public class OpenTKApp
    {
        private int         resX = 512;
        private int         resY = 512;
        private string      title = "Test App";
        private Action?     runAction = null;
        private GameWindow? window = null;

        private bool    exit = false;

        public OpenTKApp(int resX, int resY, string title)
        {
            this.resX = resX;
            this.resY = resY;
            this.title = title;
        }

        public bool Initialize()
        {
            window = new GameWindow(GameWindowSettings.Default,
                                    new NativeWindowSettings
                                    {
                                        Size = (resX, resY),
                                        Title = title,
                                        Profile = ContextProfile.Compatability,
                                        API = ContextAPI.OpenGL,
                                        Flags = ContextFlags.Default
                                    });

            window.RenderFrame += OnUpdateFrame;

            GL.Viewport(0, 0, resX, resY);

            return true;
        }

        public void Shutdown()
        {
        }


        public void Run(Action mainLoopFunction)
        {
            runAction = mainLoopFunction;

            window?.Run();
        }

        private void OnUpdateFrame(FrameEventArgs e)
        {
            if (window == null) return;

            if ((window.KeyboardState.IsKeyDown(Keys.Escape)) && (window.KeyboardState.IsKeyDown(Keys.LeftShift)))
            {
                window.Close();
            }

            runAction?.Invoke();

            window.SwapBuffers();
        }
    }
}
