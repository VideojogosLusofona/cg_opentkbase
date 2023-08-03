
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Runtime.InteropServices;

namespace OpenTKBase
{
    public class OpenTKApp
    {
        private int         resX = 512;
        private int         resY = 512;
        private string      title = "Test App";
        private bool        debug = false;
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
        private static DebugProc DebugMessageDelegate = OnDebugMessage;

        public OpenTKApp(int resX, int resY, string title, bool debug = false)
        {
            this.resX = resX;
            this.resY = resY;
            this.title = title;
            this.debug = debug;

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
                                        Flags = (debug)?(ContextFlags.Debug):(ContextFlags.Default)
                                    });

            if (debug)
            {
                GL.DebugMessageCallback(DebugMessageDelegate, IntPtr.Zero);
                GL.Enable(EnableCap.DebugOutput);
                GL.Enable(EnableCap.DebugOutputSynchronous);
            }

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
            // Set cull mode
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

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

        private static void OnDebugMessage(
            DebugSource source,     // Source of the debugging message.
            DebugType type,         // Type of the debugging message.
            int id,                 // ID associated with the message.
            DebugSeverity severity, // Severity of the message.
            int length,             // Length of the string in pMessage.
            IntPtr pMessage,        // Pointer to message string.
            IntPtr pUserParam)      // The pointer you gave to OpenGL, explained later.
        {
            // In order to access the string pointed to by pMessage, you can use Marshal
            // class to copy its contents to a C# string without unsafe code. You can
            // also use the new function Marshal.PtrToStringUTF8 since .NET Core 1.1.
            string message = Marshal.PtrToStringAnsi(pMessage, length);

            // The rest of the function is up to you to implement, however a debug output
            // is always useful.
            Console.WriteLine("[{0} source={1} type={2} id={3}] {4}", severity, source, type, id, message);

            // Potentially, you may want to throw from the function for certain severity
            // messages.
            if (type == DebugType.DebugTypeError)
            {
                throw new Exception(message);
            }
        }
    }
}
