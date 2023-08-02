
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;

namespace OpenTKBase
{
    public class OpenTKApp
    {
        private int         resX = 512;
        private int         resY = 512;
        private string      title = "Test App";
        private Action      runAction = null;
        private GameWindow  window = null;
        private Scene       _mainScene = null;
        private float       _timeDeltaTime = 0.0f;
        private Vector3     _mouseDelta;
        private int         _timeSinceLastUpdate;

        private bool    exit = false;

        public Scene    mainScene { get { return _mainScene; } }
        public float    aspectRatio { get { return (float)resX / resY; } }

        public static OpenTKApp APP;

        public OpenTKApp(int resX, int resY, string title)
        {
            this.resX = resX;
            this.resY = resY;
            this.title = title;

            APP = this;
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

            window.UpdateFrequency = 60.0f;
            window.UpdateFrame += OnUpdateFrame;
            window.RenderFrame += OnRender;

            GL.Viewport(0, 0, resX, resY);

            // Activate depth testing
            GL.Enable(EnableCap.DepthTest);
            // Set the test function
            GL.DepthFunc(DepthFunction.Lequal);
            // Enable depth write
            GL.DepthMask(true);
            // Set blend operation
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            _mainScene = new Scene();

            _timeSinceLastUpdate = System.Environment.TickCount;

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

        public void Render(RenderPipeline rp)
        {
            rp.Render(mainScene);
        }

        public bool GetKey(Keys key)
        {
            return window.KeyboardState.IsKeyDown(key);
        }

        public Vector3 GetMouseDelta()
        {
            return _mouseDelta;
        }

        public void LockMouse(bool b)
        {
            window.CursorState = (b)?(CursorState.Grabbed):(CursorState.Normal);
        }

        public float timeDeltaTime => _timeDeltaTime;

        private void OnUpdateFrame(FrameEventArgs e)
        {
            if (window == null) return;

            if (((window.KeyboardState.IsKeyDown(Keys.Escape)) && (window.KeyboardState.IsKeyDown(Keys.LeftShift))) || (exit))
            {
                window.Close();
                return;
            }

            _timeDeltaTime = (Environment.TickCount - _timeSinceLastUpdate)/1000.0f;
            _timeSinceLastUpdate = Environment.TickCount;

            var tmp = window.MouseState.Delta;
            _mouseDelta = new Vector3(tmp.X, tmp.Y, window.MouseState.ScrollDelta.Y);

            if (mainScene != null)
            {
                var allObjects = mainScene.GetAllObjects();
                foreach (var obj in allObjects)
                {
                    var allComponents = obj.GetAllComponents();
                    foreach (var c in allComponents)
                    {
                        c.Update();
                    }
                }
            }
        }

        private void OnRender(FrameEventArgs e)
        {
            if (window == null) return;

            runAction?.Invoke();

            window.SwapBuffers();
        }

    }
}
