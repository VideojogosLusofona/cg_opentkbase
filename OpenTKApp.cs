
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;

namespace SDLBase
{
    public class OpenTKApp : GameWindow
    {
        private int     resX = 512;
        private int     resY = 512;
        private string  title = "Test App";
        private Action? runAction = null;

        private bool    exit = false;

        public OpenTKApp(int resX, int resY, string title) : base(GameWindowSettings.Default, 
                                                                  new NativeWindowSettings 
                                                                  {  
                                                                      Size = (resX, resY), 
                                                                      Title = title, 
                                                                      Profile = ContextProfile.Compatability,
                                                                      API = ContextAPI.OpenGL,
                                                                      Flags = ContextFlags.Default
                                                                  })
        {
            this.resX = resX;
            this.resY = resY;
            this.title = title;
        }

        public bool Initialize()
        {
            GL.Viewport(0, 0, resX, resY);

            return true;
        }

        public void Shutdown()
        {
        }

        public void Run(Action mainLoopFunction)
        {
            runAction = mainLoopFunction;

            Run();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if ((KeyboardState.IsKeyDown(Keys.Escape)) && (KeyboardState.IsKeyDown(Keys.LeftShift)))
            {
                Close();
            }

            runAction?.Invoke();

            SwapBuffers();
        }
    }
}
